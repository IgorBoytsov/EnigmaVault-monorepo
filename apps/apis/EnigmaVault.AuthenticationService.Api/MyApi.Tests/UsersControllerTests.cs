using EnigmaVault.AuthenticationService.Api.Controllers;
using EnigmaVault.AuthenticationService.Api.DTOs.Requests;
using EnigmaVault.AuthenticationService.Api.DTOs.Responses;
using EnigmaVault.AuthenticationService.Application.Abstractions.Providers;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs;
using EnigmaVault.AuthenticationService.Application.DTOs.Commands;
using EnigmaVault.AuthenticationService.Application.DTOs.Results;
using EnigmaVault.AuthenticationService.Application.Enums;
using EnigmaVault.AuthenticationService.Application.Implementations.Providers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MyApi.Tests
{
    [TestFixture]
    internal class UsersControllerTests
    {
        private Mock<IRegisterUserUseCase> _registerUseCaseMock;
        private Mock<IAuthenticateUserUseCase> _authenticateUserUseCaseMock;
        private Mock<IDefaultErrorMessageProvider> _defaultErrorMessageProvider;
        private UsersController _controller;

        [SetUp]
        public void SetUp()
        {
            _registerUseCaseMock = new Mock<IRegisterUserUseCase>();
            _authenticateUserUseCaseMock = new Mock<IAuthenticateUserUseCase>();
            _defaultErrorMessageProvider = new Mock<IDefaultErrorMessageProvider>();

            _controller = new UsersController(_registerUseCaseMock.Object, _authenticateUserUseCaseMock.Object, new List<IDefaultErrorMessageProvider>());
        }

        private static RegisterUserApiRequest CreateSampleRegisterUserApiRequest(string? login = "LightPlay", string? userName = "Игорь", string password = "ValidPass123.!", string? email = "ValidEmail@yandex.ru", string? phone = "+7004001010")
        {
            return new RegisterUserApiRequest()
            {
                Login = login,
                UserName = userName,
                Password = password,
                Email = email,
                Phone = phone,
                IdGender = 1,
                IdCountry = 1,
            };
        }

        private static AuthenticateUserApiRequest CreateSampleAuthenticateUserApiRequest(string? login = "LightPlay", string password = "ValidPass123.!")
        {
            return new AuthenticateUserApiRequest()
            {
                Login = login,
                Password = password,
            };
        }

        private static UserDto CreateSampleUserDto(string? login = "LightPlay", string? userName = "Игорь", string password = "ValidPass123.!", string? email = "ValidEmail@yandex.ru", string? phone = "+7004001010")
        {
            return new UserDto()
            {
                IdUser = 1,
                Login = login,
                UserName = userName,
                Email = email,
                Phone = phone,
                IdGender = 1,
                IdCountry = 1,
            };
        }

        /*--Register--------------------------------------------------------------------------------------*/

        /// <summary>
        /// Вариант, если api возвращает ObjectResult, при использование return StatusCode(StatusCodes.Status201Created, result.User);
        /// Иначе нужно указывать CreatedResult вместо ObjectResult, если возврат идет через метод return Created("", result.User);
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task Register_ValidRequest_ReturnsCreatedStatus201WithUser()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest();

            var userDto = CreateSampleUserDto();
            var result = UserResult.SuccessResult(userDto);

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);

            var actionResult = await _controller.Register(apiRequest);
            var createdResult = (StatusCodeResult)actionResult;

            Assert.That(actionResult, Is.TypeOf<StatusCodeResult>(), "Должен вернуть 201 статус.");
            Assert.That(createdResult.StatusCode, Is.EqualTo(201), "Статус код должен быть 201.");
        }

        [Test]
        public async Task Register_InvalidModelState_ReturnsBadRequest()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest();
            _controller.ModelState.AddModelError("Login", "Логин уже занят");

            var actionResult = await _controller.Register(apiRequest);
            var badRequestResult = (BadRequestObjectResult)actionResult;

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>(), "Должен вернуться BadRequest (400) статус");
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400), "Статус код должен быть 400");
            Assert.That(badRequestResult.Value, Is.TypeOf<SerializableError>(), "Должен вернуть ModelState ошибку");
        }

        [Test]
        public async Task Register_ValidationFailed_ReturnsBadRequestWithValidationErrors()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest(login: " ");

            var validationErrors = new List<string> { "Вы не заполнили поле с логинов", "Пароль слишком слабый)))))", };
            var result = UserResult.ValidationFailureResult(validationErrors, "Ошибки валидации");

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.ValidationFailed)).Returns("Ошибка валидации данных");

            var actionResult = await _controller.Register(apiRequest);
            var badRequestResult = (BadRequestObjectResult)actionResult;
            var problemDetails = (ValidationProblemDetails)badRequestResult.Value!;

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(problemDetails.Title, Is.EqualTo("Ошибки валидации"));
            Assert.That(problemDetails.Errors["ValidationErrors"], Contains.Item("Вы не заполнили поле с логинов"));
            Assert.That(problemDetails.Errors["ValidationErrors"], Contains.Item("Пароль слишком слабый)))))"));
        }

        [Test]
        public async Task Register_ValidationFailed_ReturnsBadRequestWithErrorMessage()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest();

            var result = UserResult.FailureResult(ErrorCode.ValidationFailed, "Ошибки валидации");

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.ValidationFailed)).Returns("Ошибка валидации данных");

            var actionResult = await _controller.Register(apiRequest);
            var badRequestResult = (BadRequestObjectResult)actionResult;
            var errorResponse = (ErrorResponse)badRequestResult.Value!;

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("ValidationFailed"));
            Assert.That(errorResponse.Message, Is.EqualTo("Ошибки валидации"));
        }

        [Test]
        public async Task Register_LoginAlreadyTaken_ReturnsConflict()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest(login: "TestLogin");
            
            var result = UserResult.FailureResult(ErrorCode.LoginAlreadyTaken, "Логин TestLogin уже занят.");

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.LoginAlreadyTaken)).Returns("Этот логин уже занят.");

            var actionResult = await _controller.Register(apiRequest);
            var conflictRequestResult = (ConflictObjectResult)actionResult;
            var errorResponse = (ErrorResponse)conflictRequestResult.Value!;

            TestContext.Out.WriteLine(conflictRequestResult.Value);

            Assert.That(actionResult, Is.TypeOf<ConflictObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(conflictRequestResult.StatusCode, Is.EqualTo(409));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("LoginAlreadyTaken"));
            Assert.That(errorResponse.Message, Is.EqualTo("Логин TestLogin уже занят."));
        } 
        
        [Test]
        public async Task Register_EmailAlreadyRegistered_ReturnsConflict()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest(email: "TestEmail@yandex.com");
            
            var result = UserResult.FailureResult(ErrorCode.EmailAlreadyRegistered, "Почта TestEmail@yandex.com уже занята.");

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.EmailAlreadyRegistered)).Returns("Эта почта уже занята.");

            var actionResult = await _controller.Register(apiRequest);
            var conflictRequestResult = (ConflictObjectResult)actionResult;
            var errorResponse = (ErrorResponse)conflictRequestResult.Value!;

            TestContext.Out.WriteLine(conflictRequestResult.Value);

            Assert.That(actionResult, Is.TypeOf<ConflictObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(conflictRequestResult.StatusCode, Is.EqualTo(409));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("EmailAlreadyRegistered"));
            Assert.That(errorResponse.Message, Is.EqualTo("Почта TestEmail@yandex.com уже занята."));
        } 
        
        [Test]
        public async Task Register_PhoneAlreadyRegistered_ReturnsConflict()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest(phone: "80004001010");
            
            var result = UserResult.FailureResult(ErrorCode.PhoneAlreadyRegistered, "Номер 80004001010 уже используется.");

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.PhoneAlreadyRegistered)).Returns("Номер уже используется.");

            var actionResult = await _controller.Register(apiRequest);
            var conflictRequestResult = (ConflictObjectResult)actionResult;
            var errorResponse = (ErrorResponse)conflictRequestResult.Value!;

            TestContext.Out.WriteLine(conflictRequestResult.Value);

            Assert.That(actionResult, Is.TypeOf<ConflictObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(conflictRequestResult.StatusCode, Is.EqualTo(409));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("PhoneAlreadyRegistered"));
            Assert.That(errorResponse.Message, Is.EqualTo("Номер 80004001010 уже используется."));
        }

        [Test]
        public async Task Register_WeakPassword_ReturnsBadRequest()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest(password: "12345");

            var result = UserResult.FailureResult(ErrorCode.WeakPassword, "Ваш пароль слишком легкий.");

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.WeakPassword)).Returns("Недостаточная безопасность пароля.");

            var actionResult = await _controller.Register(apiRequest);
            var badRequestResult = (BadRequestObjectResult)actionResult;
            var errorResponse = (ErrorResponse)badRequestResult.Value!;

            TestContext.Out.WriteLine(badRequestResult.Value);

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("WeakPassword"));
            Assert.That(errorResponse.Message, Is.EqualTo("Ваш пароль слишком легкий."));
        }

        [Test]
        public async Task Register_InvalidRole_ReturnsBadRequest()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest();

            var result = UserResult.FailureResult(ErrorCode.InvalidRole, "Не верно указанная роль в системе.");

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.InvalidRole)).Returns("В запросе пришла не верная роль.");

            var actionResult = await _controller.Register(apiRequest);
            var badRequestResult = (BadRequestObjectResult)actionResult;
            var errorResponse = (ErrorResponse)badRequestResult.Value!;

            TestContext.Out.WriteLine(badRequestResult.Value);

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("InvalidRole"));
            Assert.That(errorResponse.Message, Is.EqualTo("Не верно указанная роль в системе."));
        }    
        
        [Test]
        public async Task Register_InvalidAccountStatus_ReturnsBadRequest()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest();

            var result = UserResult.FailureResult(ErrorCode.InvalidAccountStatus, "Не верно указанный статус аккаунта в системе.");

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.InvalidAccountStatus)).Returns("В запросе пришел не верный статус акаунта.");

            var actionResult = await _controller.Register(apiRequest);
            var badRequestResult = (BadRequestObjectResult)actionResult;
            var errorResponse = (ErrorResponse)badRequestResult.Value!;

            TestContext.Out.WriteLine(badRequestResult.Value);

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("InvalidAccountStatus"));
            Assert.That(errorResponse.Message, Is.EqualTo("Не верно указанный статус аккаунта в системе."));
        } 
        
        [Test]
        public async Task Register_DomainCreationError_ReturnsBadRequest()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest();

            var result = UserResult.FailureResult(ErrorCode.DomainCreationError, "Произошла ошибка при создание данных.");

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.DomainCreationError)).Returns("Произошла ошибка при создание домен данных.");

            var actionResult = await _controller.Register(apiRequest);
            var badRequestResult = (BadRequestObjectResult)actionResult;
            var errorResponse = (ErrorResponse)badRequestResult.Value!;

            TestContext.Out.WriteLine(badRequestResult.Value);

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("DomainCreationError"));
            Assert.That(errorResponse.Message, Is.EqualTo("Произошла ошибка при создание данных."));
        }
        
        [Test]
        public async Task Register_SaveUserError_ReturnsBadRequest()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest();

            var result = UserResult.FailureResult(ErrorCode.SaveUserError);

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.SaveUserError)).Returns("Произошла ошибка при сохранение данных в хранилище.");

            var actionResult = await _controller.Register(apiRequest);
            var badObjectResult = (ObjectResult)actionResult;
            var errorResponse = (ErrorResponse)badObjectResult.Value!;

            TestContext.Out.WriteLine(badObjectResult.Value);

            Assert.That(actionResult, Is.TypeOf<ObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(badObjectResult.StatusCode, Is.EqualTo(500));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("SaveUserError"));
            Assert.That(errorResponse.Message, Is.EqualTo("Во время регистрации произошла непредвиденная ошибка. Пожалуйста, попробуйте позже."));
        }
        
        [Test]
        public async Task Register_UnknownError_ReturnsBadRequest()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest();

            var result = UserResult.FailureResult(ErrorCode.UnknownError);

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.UnknownError)).Returns("Произошла неизвестная ошибка!.");

            var actionResult = await _controller.Register(apiRequest);
            var badObjectResult = (ObjectResult)actionResult;
            var errorResponse = (ErrorResponse)badObjectResult.Value!;

            TestContext.Out.WriteLine(badObjectResult.Value);

            Assert.That(actionResult, Is.TypeOf<ObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(badObjectResult.StatusCode, Is.EqualTo(500));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("UnknownError"));
            Assert.That(errorResponse.Message, Is.EqualTo("Во время регистрации произошла непредвиденная ошибка. Пожалуйста, попробуйте позже."));
        }

        [Test]
        public async Task Register_None_ReturnsBadRequest()
        {
            var apiRequest = CreateSampleRegisterUserApiRequest();

            var result = UserResult.FailureResult(ErrorCode.None);

            _registerUseCaseMock.Setup(u => u.RegisterAsync(It.IsAny<RegisterUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.None)).Returns("Произошла непредвиденная ошибка!!.");

            var actionResult = await _controller.Register(apiRequest);
            var badObjectResult = (ObjectResult)actionResult;
            var errorResponse = (ErrorResponse)badObjectResult.Value!;

            TestContext.Out.WriteLine(badObjectResult.Value);

            Assert.That(actionResult, Is.TypeOf<ObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(badObjectResult.StatusCode, Is.EqualTo(500));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("None"));
            Assert.That(errorResponse.Message, Is.EqualTo("Во время регистрации произошла непредвиденная ошибка. Пожалуйста, попробуйте позже."));
        }

        /*--Authenticate----------------------------------------------------------------------------------*/

        [Test]
        public async Task Authenticate_ValidRequest_ReturnsStatus200OKWithUserAuthenticateResponse()
        {
            var apiRequest = CreateSampleAuthenticateUserApiRequest();

            var userDto = CreateSampleUserDto();
            var result = UserResult.SuccessResult(userDto);

            _authenticateUserUseCaseMock.Setup(u => u.Authenticate(It.IsAny<AuthenticateUserCommand>())).ReturnsAsync(result);

            var actionResult = await _controller.Authenticate(apiRequest);
            var authResult = (ObjectResult)actionResult;
            var authResponse = (UserAuthenticateResponse?)authResult.Value;

            TestContext.Out.WriteLine(authResponse!.Login);

            Assert.That(actionResult, Is.TypeOf<ObjectResult>(), "Должен вернуться ObjectResult.");
            Assert.That(authResult.StatusCode, Is.EqualTo(200), "Статус код должен быть 200.");
            Assert.That(authResult.Value, Is.Not.Null, "Значение не должно быть пустым");
            Assert.That(authResponse.Login, Is.EqualTo("LightPlay"), "Значение должно быть LightPlay");
        }

        [Test]
        public async Task Authenticate_InvalidModelState_ReturnsBadRequest()
        {
            var apiRequest = CreateSampleAuthenticateUserApiRequest();
            _controller.ModelState.AddModelError("Login", "Логин не найден");

            var actionResult = await _controller.Authenticate(apiRequest);
            var badRequestResult = (BadRequestObjectResult)actionResult;

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>(), "Должен вернуться BadRequest (400) статус");
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400), "Статус код должен быть 400");
            Assert.That(badRequestResult.Value, Is.TypeOf<SerializableError>(), "Должен вернуть ModelState ошибку");
        }

        [Test]
        public async Task Authenticate_ValidationFailed_ReturnsBadRequestWithValidationErrors()
        {
            var apiRequest = CreateSampleAuthenticateUserApiRequest(login: " ");

            var validationErrors = new List<string> { "Вы не заполнили поле с логинов", "Пароль слишком слабый)))))", };
            var result = UserResult.ValidationFailureResult(validationErrors, "Ошибки валидации");

            _authenticateUserUseCaseMock.Setup(u => u.Authenticate(It.IsAny<AuthenticateUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.ValidationFailed)).Returns("Ошибка валидации данных");

            var actionResult = await _controller.Authenticate(apiRequest);
            var badRequestResult = (BadRequestObjectResult)actionResult;
            var problemDetails = (ValidationProblemDetails)badRequestResult.Value!;

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(problemDetails.Title, Is.EqualTo("Ошибки валидации"));
            Assert.That(problemDetails.Errors["ValidationErrors"], Contains.Item("Вы не заполнили поле с логинов"));
            Assert.That(problemDetails.Errors["ValidationErrors"], Contains.Item("Пароль слишком слабый)))))"));
        }

        [Test]
        public async Task Authenticate_LoginNotExist_ReturnsConflict()
        {
            var apiRequest = CreateSampleAuthenticateUserApiRequest();

            var result = UserResult.FailureResult(ErrorCode.LoginNotExist, "Логина TestLogin не существует.");

            _authenticateUserUseCaseMock.Setup(u => u.Authenticate(It.IsAny<AuthenticateUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.LoginNotExist)).Returns("Этот логин уже занят.");

            var actionResult = await _controller.Authenticate(apiRequest);
            var conflictRequestResult = (ConflictObjectResult)actionResult;
            var errorResponse = (ErrorResponse)conflictRequestResult.Value!;

            TestContext.Out.WriteLine(conflictRequestResult.Value);

            Assert.That(actionResult, Is.TypeOf<ConflictObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(conflictRequestResult.StatusCode, Is.EqualTo(409));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("LoginNotExist"));
            Assert.That(errorResponse.Message, Is.EqualTo("Логина TestLogin не существует."));
        }

        [Test]
        public async Task Authenticate_InvalidPassword_ReturnsBadRequest()
        {
            var apiRequest = CreateSampleAuthenticateUserApiRequest();

            var result = UserResult.FailureResult(ErrorCode.InvalidPassword, "Пароль не совпадает.");

            _authenticateUserUseCaseMock.Setup(u => u.Authenticate(It.IsAny<AuthenticateUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.InvalidPassword)).Returns("Пароли не равны.");

            var actionResult = await _controller.Authenticate(apiRequest);
            var conflictRequestResult = (BadRequestObjectResult)actionResult;
            var errorResponse = (ErrorResponse)conflictRequestResult.Value!;

            TestContext.Out.WriteLine(conflictRequestResult.Value);

            Assert.That(actionResult, Is.TypeOf<BadRequestObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(conflictRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("InvalidPassword"));
            Assert.That(errorResponse.Message, Is.EqualTo("Пароль не совпадает."));
        }

        [Test]
        public async Task Authenticate_UnknownError_ReturnsBadRequest()
        {
            var apiRequest = CreateSampleAuthenticateUserApiRequest();

            var result = UserResult.FailureResult(ErrorCode.UnknownError);

            _authenticateUserUseCaseMock.Setup(u => u.Authenticate(It.IsAny<AuthenticateUserCommand>())).ReturnsAsync(result);
            _defaultErrorMessageProvider.Setup(p => p.GetMessage(ErrorCode.UnknownError)).Returns("Произошла неизвестная ошибка!.");

            var actionResult = await _controller.Authenticate(apiRequest);
            var badObjectResult = (ObjectResult)actionResult;
            var errorResponse = (ErrorResponse)badObjectResult.Value!;

            TestContext.Out.WriteLine(badObjectResult.Value);

            Assert.That(actionResult, Is.TypeOf<ObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(badObjectResult.StatusCode, Is.EqualTo(500));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("UnknownError"));
            Assert.That(errorResponse.Message, Is.EqualTo("Во время аутентификации произошла непредвиденная ошибка. Пожалуйста, попробуйте позже."));
        }
        
        [Test]
        public async Task Authenticate_UnknownError_ThrowsException()
        {
            var apiRequest = CreateSampleAuthenticateUserApiRequest();

            _authenticateUserUseCaseMock.Setup(u => u.Authenticate(It.IsAny<AuthenticateUserCommand>())).Throws( new ArgumentNullException());

            var actionResult = await _controller.Authenticate(apiRequest);
            var badObjectResult = (ObjectResult)actionResult;
            var errorResponse = (ErrorResponse)badObjectResult.Value!;

            TestContext.Out.WriteLine(badObjectResult.Value);

            Assert.That(actionResult, Is.TypeOf<ObjectResult>());
            Assert.That(errorResponse, Is.TypeOf<ErrorResponse>());
            Assert.That(badObjectResult.StatusCode, Is.EqualTo(500));
            Assert.That(errorResponse.ErrorCode, Is.EqualTo("UnknownError"));
            Assert.That(errorResponse.Message, Is.EqualTo("Во время аутентификации произошла непредвиденная ошибка. Пожалуйста, попробуйте позже либо обратитесь в поддержку"));
        }
    }
}