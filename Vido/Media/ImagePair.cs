// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Media
{
  public class ImagePair
  {
    #region Public Properties
    public IImageHolder First { get; set; }
    public IImageHolder Second { get; set; }
    #endregion

    #region Public Constructors
    public ImagePair()
    {
      this.First = null;
      this.Second = null;
    }
    #endregion
  }
}