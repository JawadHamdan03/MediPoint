using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Common.Exceptions;

public class WrongPasswordException(string password) : Exception($"Wrong Password {password} ")
{
}
