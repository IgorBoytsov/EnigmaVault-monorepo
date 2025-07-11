﻿using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.SecretCase
{
    public interface IDeleteSecretUseCase
    {
        Task<Result<int?>> DeleteAsync(int idSecret);
    }
}