﻿using AutoMapper;
using BC = BCrypt.Net.BCrypt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Cookwi.Common.Exceptions;
using Cookwi.Db.Entities;
using Cookwi.Db;
using Microsoft.EntityFrameworkCore;
using Cookwi.Api.Models.Accounts;
using Cookwi.Api.Helpers;
using Cookwi.Common.Models;

namespace Cookwi.Api.Services
{
    public interface IAccountService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        void Register(RegisterRequest model, string origin);
        void VerifyEmail(string token);
        void ForgotPassword(ForgotPasswordRequest model, string origin);
        void ValidateResetToken(ValidateResetTokenRequest model);
        void ResetPassword(ResetPasswordRequest model);
        IEnumerable<AccountResponse> GetAll();
        AccountResponse GetById(int id);
        AccountResponse Create(CreateAccountRequest model);
        AccountResponse Update(int id, UpdateRequest model);
        void Delete(int id);
    }

    public class AccountService : IAccountService
    {
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IEmailService _emailService;
        private readonly CookwiContext _ctx;

        public AccountService(
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IEmailService emailService,
            CookwiContext ctx)
        {
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _ctx = ctx;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var account = _ctx.Accounts.Include(a => a.RefreshTokens).FirstOrDefault(a => a.Email == model.Email);

            if (account == null || !account.IsVerified || !BC.Verify(model.Password, account.PasswordHash))
            {
                throw new BadRequestException("Email or password is incorrect");
            }

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(account);
            var refreshToken = generateRefreshToken(ipAddress);
            account.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from account
            removeOldRefreshTokens(account);

            // save changes to db
            _ctx.SaveChanges();

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = refreshToken.Token;

            return response;
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var (refreshToken, account) = getRefreshToken(token);

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            account.RefreshTokens.Add(newRefreshToken);

            removeOldRefreshTokens(account);

            _ctx.SaveChanges();

            // generate new jwt
            var jwtToken = generateJwtToken(account);

            var response = _mapper.Map<AuthenticateResponse>(account);
            response.JwtToken = jwtToken;
            response.RefreshToken = newRefreshToken.Token;

            return response;
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var (refreshToken, account) = getRefreshToken(token);

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _ctx.SaveChanges();
        }

        public void Register(RegisterRequest model, string origin)
        {
            _ctx.Database.EnsureCreated();
            // validate
            if (_ctx.Accounts.FirstOrDefault(a => a.Email == model.Email) != null)
            {
                // send already registered error in email to prevent account enumeration
                sendAlreadyRegisteredEmail(model.Email, origin);
                return;
            }

            // map model to new account object
            var account = _mapper.Map<Account>(model);

            // first registered account is an admin
            var isFirstAccount = _ctx.Accounts.Count() == 0;
            account.Roles = new List<Role> { Role.User };
            if (isFirstAccount) account.Roles.Add(Role.Admin);
            account.VerificationToken = randomTokenString();

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _ctx.Accounts.Add(account);
            _ctx.SaveChanges();

            // send email
            sendVerificationEmail(account, origin);
        }

        public void VerifyEmail(string token)
        {
            var account = _ctx.Accounts.FirstOrDefault(a => a.VerificationToken == token);

            if (account == null) throw new BadRequestException("Verification failed");

            account.Verified = DateTime.UtcNow;
            account.VerificationToken = null;

            _ctx.SaveChanges();
        }

        public void ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = getAccount(model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            // create reset token that expires after 1 day
            account.ResetToken = randomTokenString();
            account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

            _ctx.SaveChanges();

            // send email
            sendPasswordResetEmail(account, origin);
        }

        public void ValidateResetToken(ValidateResetTokenRequest model)
        {
            var account = _ctx.Accounts.FirstOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
            {
                throw new BadRequestException("Invalid token");
            }
        }

        public void ResetPassword(ResetPasswordRequest model)
        {
            var account = _ctx.Accounts.FirstOrDefault(x =>
                x.ResetToken == model.Token &&
                x.ResetTokenExpires > DateTime.UtcNow);

            if (account == null)
            {
                throw new BadRequestException("Invalid token");
            }

            // update password and remove reset token
            account.PasswordHash = BC.HashPassword(model.Password);
            account.PasswordReset = DateTime.UtcNow;
            account.ResetToken = null;
            account.ResetTokenExpires = null;

            _ctx.SaveChanges();
        }

        public IEnumerable<AccountResponse> GetAll()
        {
            var accounts = _ctx.Accounts.AsEnumerable();

            return _mapper.Map<IList<AccountResponse>>(accounts);
        }

        public AccountResponse GetById(int id)
        {
            var account = getAccount(id);

            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Create(CreateAccountRequest model)
        {
            // validate
            if (getAccount(model.Email) != null)
            {
                throw new BadRequestException($"Email '{model.Email}' is already registered");
            }

            // map model to new account object
            var account = _mapper.Map<Account>(model);
            account.Created = DateTime.UtcNow;
            account.Verified = DateTime.UtcNow;

            // hash password
            account.PasswordHash = BC.HashPassword(model.Password);

            // save account
            _ctx.Add(account);
            _ctx.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public AccountResponse Update(int id, UpdateRequest model)
        {
            var account = getAccount(id);

            // validate
            if (account.Email != model.Email && getAccount(model.Email) != null)
            {
                throw new BadRequestException($"Email '{model.Email}' is already taken");
            }

            // hash password if it was entered
            if (!string.IsNullOrEmpty(model.Password))
            {
                account.PasswordHash = BC.HashPassword(model.Password);
            }

            // copy model to account and save
            _mapper.Map(model, account);
            account.Updated = DateTime.UtcNow;

            _ctx.SaveChanges();

            return _mapper.Map<AccountResponse>(account);
        }

        public void Delete(int id)
        {
            var account = getAccount(id);
            _ctx.Accounts.Remove(account);
        }

        // helper methods

        private Account getAccount(int id)
        {
            var account = _ctx.Accounts.Include(a => a.RefreshTokens).FirstOrDefault(a => a.Id == id);
            if (account == null)
            {
                throw new NotFoundException("Account not found");
            }

            return account;
        }

        private Account getAccount(string email)
        {
            var account = _ctx.Accounts.Include(a => a.RefreshTokens).FirstOrDefault(a => a.Email == email);
            if (account == null)
            {
                throw new NotFoundException($"Account with email address {email} does not exist");
            }

            return account;
        }

        private (RefreshToken, Account) getRefreshToken(string token)
        {
            var account = _ctx.Accounts.Include(a => a.RefreshTokens)
                .FirstOrDefault(a => a.RefreshTokens.Any(rt => rt.Token == token));

            if (account == null)
            {
                throw new BadRequestException("Invalid token");
            }

            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive)
            {
                throw new BadRequestException("Invalid token");
            }

            return (refreshToken, account);
        }

        private string generateJwtToken(Account account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Security.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = randomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        private void removeOldRefreshTokens(Account account)
        {
            var refreshTokens = account.RefreshTokens.Where(x => !x.IsActive && x.Created.AddDays(_appSettings.Security.JwtTTL) <= DateTime.UtcNow);
            _ctx.RefreshTokens.RemoveRange(refreshTokens);
        }

        private string randomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);

            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private void sendVerificationEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
                message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{account.VerificationToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
            );
        }

        private void sendAlreadyRegisteredEmail(string email, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
                message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
            else
                message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

            _emailService.Send(
                to: email,
                subject: "Sign-up Verification API - Email Already Registered",
                html: $@"<h4>Email Already Registered</h4>
                         <p>Your email <strong>{email}</strong> is already registered.</p>
                         {message}"
            );
        }

        private void sendPasswordResetEmail(Account account, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{account.ResetToken}</code></p>";
            }

            _emailService.Send(
                to: account.Email,
                subject: "Sign-up Verification API - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }
    }
}
