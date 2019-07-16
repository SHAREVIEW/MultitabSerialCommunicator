﻿using MultitabSerialCommunicator.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MultitabSerialCommunicator
{
    public class SerialViewModel : BaseViewModel
    {
        #region Private fields
        private int baudRate;
        private string portName;
        private string dataBits;
        private string stopBits;
        private string parity;
        private string handShake;
        private ObservableCollection<string> ports = new ObservableCollection<string>();
        private string mainText;
        private string sendText;
        private string btnText;
        private int readTimeout;
        private int writeTimeout;
        private int bufferSize;
        private bool dtrEnable;
        private SerialDev serialDev;// = new SerialDev();
        readonly ISerialModel iSerial;
        public SerialDataCollections SerialDataCollections { get; set; } = new SerialDataCollections();
        #endregion

        #region public fields
        public int    SVMBaudRate                 { get { return baudRate;  }    set { baudRate = value;     RaisePropertyChanged(); } }
        public string SVMPortName                 { get { return portName;  }    set { portName = value;     serialDev.SetPortName(value);  RaisePropertyChanged(); } }

        internal void AddNewMessage(object data, object rxortx)
        {
            throw new NotImplementedException();
        }

        public string SVMDataBits                 { get { return dataBits;  }    set { dataBits = value;     RaisePropertyChanged(); } }
        public string SVMStopbits                 { get { return stopBits;  }    set { stopBits = value;     RaisePropertyChanged(); } }
        public string SVMParity                   { get { return parity;    }    set { parity = value;       RaisePropertyChanged(); } }
        public string SVMHandShake                { get { return handShake; }    set { handShake = value;    RaisePropertyChanged(); } }
        public ObservableCollection<string> Ports { get { return ports;     }    set { ports = value;        RaisePropertyChanged(); } }
        public string MainText                    { get { return mainText; }     set { mainText = value;     RaisePropertyChanged(); } }
        public string SendText                    { get { return sendText; }     set { sendText = value;     RaisePropertyChanged(); } }
        public string ButtonText                  { get { return btnText; }      set { btnText = value;      RaisePropertyChanged(); } }
        public int ReadTimeout                    { get { return readTimeout; }  set { readTimeout = value;  RaisePropertyChanged(); } }
        public int WriteTimeout                   { get { return writeTimeout; } set { writeTimeout = value; RaisePropertyChanged(); } }
        public int BufferSize                     { get { return bufferSize; }   set { bufferSize = value;   RaisePropertyChanged(); } }
        public bool DTREnable                     { get { return dtrEnable; }    set { dtrEnable = value;    serialDev.UpdateDTR(value); RaisePropertyChanged(); } }
        public ICommand ConnectToPort     { get; set; }
        public ICommand SendSerialMessage { get; set; }
        public ICommand RefreshCOMsList   { get; set; }
        public ICommand ClearBuffers      { get; set; }
        public ICommand ClearText         { get; set; }
        #endregion

        #region Constructor

        public SerialViewModel(SerialDev serialDev)
        {
            this.serialDev = serialDev;
            iSerial = this.serialDev;
            iSerial.OnMessage = message;
            ConnectToPort         = new DelegateCommand(connect);
            SendSerialMessage     = new DelegateCommand(sendMessage);
            RefreshCOMsList       = new DelegateCommand(refreshList);
            ClearBuffers          = new DelegateCommand(clrBuffers);
            ClearText             = new DelegateCommand(clrMessageBuffer);
            SVMBaudRate           = 115200;
            SVMDataBits           = "8";
            SVMStopbits           = "One";
            SVMParity             = "None";
            SVMHandShake          = "None";
            ButtonText            = "Connect";
            ReadTimeout           = 500;
            WriteTimeout          = 500;
            BufferSize            = 4096;
            this.serialDev.SetPortValues(SVMBaudRate.ToString(),
                                    SVMDataBits,
                                    SVMStopbits,
                                    SVMParity,
                                    SVMHandShake,
                                    Encoding.ASCII,
                                    "",
                                    BufferSize);
            this.serialDev.SetTimeouts(ReadTimeout, WriteTimeout);
            refreshList();
        }

        #endregion

        #region Methods

        private void message(string data, string rxortx)
        {
            AddNewMessage(data, rxortx);
        }

        private void clrMessageBuffer()
        {
            MainText = "";
        }
        private void clrBuffers()
        {
            serialDev.ClearBuffers();
        }

        private void connect()
        {
            ButtonText = serialDev.AutoConnectToArduino();
        }

        private void sendMessage()
        {
            serialDev.SendSerialMessage(SendText);
        }

        private void refreshList()
        {
            Ports.Clear();
            foreach (string v in SerialPort.GetPortNames()) Ports.Add(v);
        }

        public void AddNewMessage(string data, string RXorTX)
        {
            MainText += $"{RXorTX}> {data}" + '\n';
        }

        public void DisposeProcedure()
        {
            serialDev.DisposeProc();
            serialDev = null;
        }

        #endregion
    }
}
