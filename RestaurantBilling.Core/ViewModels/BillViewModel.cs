﻿using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using RestaurantBilling.Core.Models;
using RestaurantBilling.Core.Services;
using System.Windows.Input;

namespace RestaurantBilling.Core.ViewModels
{
    /// <summary>
	/// All view models inherit from MvxViewModel in MVVMCross
	/// </summary>
	public class BillViewModel : MvxViewModel
    {
        readonly IBillCalculator _calculation;
        Bill _bill;
        int _gratuity;

        public string CustomerEmail
        {
            get { return _bill.CustomerEmail; }
            set {
                _bill.CustomerEmail = value;
                RaisePropertyChanged(() => CustomerEmail);
            }
        }

        public double SubTotal
        {
            get { return _bill.SubTotal; }
            set {
                _bill.SubTotal = value;
                RaisePropertyChanged(() => SubTotal);
                Recalculate();
            }
        }

        public int Gratuity
        {
            get { return _gratuity; }
            set {
                _gratuity = value;
                RaisePropertyChanged(() => Gratuity);
                Recalculate();
            }
        }

        public double Tip
        {
            get { return _bill.Tip; }
            set {
                _bill.Tip = value;
                RaisePropertyChanged(() => Tip);
            }
        }

        public double Total
        {
            get { return _bill.AmountPaid; }
            set {
                _bill.AmountPaid = value;
                RaisePropertyChanged(() => Total);
            }
        }

        /// <summary>
        /// Use constructor injection to supply _calculation with the implementation.
        /// </summary>
        /// <param name="calculation"></param>
        public BillViewModel(IBillCalculator calculation)
        {
            _calculation = calculation;
        }

        public ICommand NavBack
        {
            get {
                return new MvxCommand(() => Close(this));
            }
        }

        public ICommand SaveBill
        {
            get {
                return new MvxCommand(() => {
                    if(_bill.IsValid()) {
                        // Here we are simply waiting for the thread to complete.
                        // In a production app, this would be the opportunity to
                        // provide UI updates while the save thread completes.
                        Mvx.Resolve<Repository>().CreateBill(_bill).Wait();
                        Close(this);
                    }
                });
            }
        }

        public void Init(Bill bill = null)
        {
            _bill = bill == null ? new Bill() : bill;
            _gratuity = (int)_calculation.Gratuity(_bill.SubTotal, bill.Tip);
            RaiseAllPropertiesChanged();
        }

        void Recalculate()
        {
            Tip = _calculation.TipAmount(SubTotal, Gratuity);
            Total = _calculation.BillTotal(SubTotal, Gratuity);
        }
    }
}
