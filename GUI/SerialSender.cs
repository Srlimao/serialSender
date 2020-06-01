﻿using System;
using System.Globalization;
using System.IO.Ports;
using OpenHardwareMonitor.Hardware;

namespace OpenHardwareMonitor.GUI {

  class SerialSender {
    public SerialPort SelectedSerialPort;
    byte _cpuTemp = 0;
    byte _gpuTemp = 0;
    byte _gpuLoad = 0;
    byte _cpuLoad = 0;
    byte _ramUsed = 0;
    private byte[] data;







    public void Selected_Serial(object sender, EventArgs e, string selectedPort) {
      if (SelectedSerialPort == null) {
        SelectedSerialPort = new SerialPort(selectedPort);
        if (!SelectedSerialPort.IsOpen) {
          SelectedSerialPort.Open();
        }
      } else if (selectedPort == SelectedSerialPort.PortName) {
        SelectedSerialPort.Close();
        SelectedSerialPort = null;
      }
    }

    public void DataSend(Computer thisComputer) {

      if (SelectedSerialPort == null || !SelectedSerialPort.IsOpen)
        return;

      foreach (IHardware hw in thisComputer.Hardware) {
        hw.Update();
        foreach (ISensor s in hw.Sensors) {
          switch (s.SensorType) {

            case SensorType.Temperature:

              if (s.Value != null) {
                int curTemp = (int)s.Value;
                switch (s.Name) {
                  case "CPU Package":
                    _cpuTemp = Convert.ToByte(curTemp);
                    break;
                  case "GPU Core":
                    _gpuTemp = Convert.ToByte(curTemp);
                    break;
                }
              }
              break;

            case SensorType.Load:

              if (s.Value != null) {
                int curLoad = (int)s.Value;
                switch (s.Name) {
                  case "CPU Total":
                    _cpuLoad = Convert.ToByte(curLoad);
                    break;
                  case "GPU Core":
                    _gpuLoad = Convert.ToByte(curLoad);
                    break;
                }
              }

              break;
            case SensorType.Data:

              if (s.Value != null) {
                switch (s.Name) {
                  case "Used Memory":
                    int intRam = (int)(s.Value*10);
                    _ramUsed = Convert.ToByte(intRam);
                    break;
                }
              }

              break;

            default:
              break;
          }
        }
      }

      string arduinoData = "C" + _cpuTemp + "c " + _cpuLoad + "%|G" + _gpuTemp + "c " + _gpuLoad + "%|R" + _ramUsed + "G|";

      data = new[] {_cpuTemp, _gpuTemp, _cpuLoad, _gpuLoad, _ramUsed};
      Console.WriteLine(arduinoData);
      SelectedSerialPort.WriteLine(arduinoData);
    }
  }
}
