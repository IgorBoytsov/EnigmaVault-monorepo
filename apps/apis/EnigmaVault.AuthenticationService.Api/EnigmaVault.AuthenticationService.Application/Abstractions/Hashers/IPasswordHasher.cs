﻿using EnigmaVault.AuthenticationService.Application.DTOs.CryptoParameters;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.Hashers
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHashString);
        CryptoParameter GetParametersFromHash(string storedHashString);
    }
}