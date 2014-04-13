// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  public class UserData : IUserData
  {
    string IUserData.UserData { get; set; }

    public UserData(string userData)
    {
      (this as IUserData).UserData = userData;
    }
  }
}
