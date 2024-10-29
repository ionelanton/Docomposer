using System;

namespace Docomposer.Blazor.Services
{
    public class BlazorDataTransferService
    {
        private BlazorTransferData _blazorTransferData;
       
        public BlazorTransferData TransferData
        {
            set
            {
                _blazorTransferData = value;
                NotifyDragAndDropDataChanged();
            }
        }

        public event EventHandler<BlazorTransferData> OnDataChanged;

        private void NotifyDragAndDropDataChanged()
        {
            OnDataChanged?.Invoke(this, _blazorTransferData);
        }
    }
}