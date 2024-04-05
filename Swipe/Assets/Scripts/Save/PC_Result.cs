﻿using Alekrus.UnivarsalPlatform;

namespace Assets.Scripts.Save
{
  internal class PC_Result : IResult
  {
    public bool Result { get; }

    public PC_Result(bool parResult)
    {
      Result = parResult;
    }

    public bool IsSuccess()
    {
      return Result;
    }

    public override string ToString()
    {
      return Result.ToString();
    }
  }
}