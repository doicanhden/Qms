// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  public class SavedImagesEventArgs : System.EventArgs
  {
    #region Public Properties
    public IFileStorage FileStorage { get; set; }
    public string FirstImage { get; set; }
    public string SecondImage { get; set; }
    #endregion
  }
}