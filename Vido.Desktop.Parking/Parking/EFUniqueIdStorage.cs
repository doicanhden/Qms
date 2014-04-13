namespace Vido.Parking
{
  using System;
  using System.Diagnostics;
  using System.Linq;
  using Vido.Qms;

  public class EFUniqueIdStorage : IUniqueIdStorage, IDisposable
  {
    #region Data Members
    private readonly object locker = new object();
    private readonly VidoParkingEntities entities;
    #endregion

    #region Public Events
    /// <summary>
    /// Sự kiện kích hoạt khi nhận được thông báo mới từ Bãi.
    /// </summary>
    public event EventHandler NewMessage;
    #endregion

    #region Public Constructors
    public EFUniqueIdStorage()
    {
      this.entities = new VidoParkingEntities();
    }
    #endregion

    #region Private Methods
    private void RaiseNewMessage(string message)
    {
      if (NewMessage != null)
      {
        NewMessage(this, new NewMessageEventArgs()
        {
          Message = message
        });
      }
    }
    #endregion

    #region Implementation of IUniqueIdStorage

    /// <summary>
    /// Kiểm tra thẻ có tồn tại và đang được sử dụng hay không.
    /// </summary>
    /// <param name="uniqueId">Dữ liệu thẻ</param>
    /// <returns></returns>
    public bool CanUse(string uniqueId)
    {
      try
      {
        var cards = from Cards in entities.Card
                    where
                      Cards.CardId == uniqueId
                    select Cards;

        return (cards.Count() == 1 && cards.ToArray()[0].State == 0);
      }
      catch (Exception ex)
      {
        Debug.WriteLine("EFUniqueIdStorage.CanUse: " + ex.Message);
        RaiseNewMessage("EFUniqueIdStorage.CanUse: " + ex.Message);
        return (false);
      }
    }
    public bool Insert(string uniqueId)
    {
      throw new NotImplementedException();
    }
    public bool Remove(string uniqueId)
    {
      throw new NotImplementedException();
    }
    public UniqueIdState Status(string uniqueId)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Implementation of IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        // dispose managed resources
        entities.Dispose();
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion
  }
}