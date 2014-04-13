// Copyright (C) 2014 Vido's R&D.  All rights reserved.

namespace Vido.Qms
{
  public interface IEntryRecorder
  {
    #region Properties
    /// <summary>
    /// Số vị trí tối thiểu
    /// </summary>
    int MinimumSlots { get; set; }

    /// <summary>
    /// Số vị trí tối đa.
    /// </summary>
    int MaximumSlots { get; set; }

    /// <summary>
    /// Trạng thái đầy.
    /// </summary>
    bool IsFull { get; }
    #endregion

    #region Methods
    bool CanImport(string uniqueId, string userData, out IImport import);
    bool Import(Entry entry);

    bool CanExport(string uniqueId, string userData, out IExport export);
    bool Export(Entry entry);

    void Blocked(Entry entry);
    #endregion
  }
}