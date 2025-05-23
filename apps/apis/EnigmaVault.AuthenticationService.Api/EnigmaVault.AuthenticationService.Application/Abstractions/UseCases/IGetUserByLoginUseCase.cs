﻿using EnigmaVault.AuthenticationService.Application.DTOs.Results;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.UseCases
{
    public interface IGetUserByLoginUseCase
    {
        Task<UserResult> GetUserByLoginAsync(string login);
    }
}