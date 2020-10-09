using OXGaming.TibiaAPI;
using OXGaming.TibiaAPI.Constants;
using OXGaming.TibiaAPI.Utilities;
using Sniffer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Sniffer.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        #region TibiaApi
        private Client _client;

        #endregion
        private ObservableCollection<Packet> _packets;
        public ObservableCollection<Packet> Packets
        {
            get { return _packets; }
            set
            {
                _packets = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<Packet> _filteredPackets;
        public ObservableCollection<Packet> FilteredPackets
        {
            get { return _filteredPackets; }
            set
            {
                _filteredPackets = value;
                OnPropertyChanged();
            }
        }

        private Packet _selectedPacket;
        public Packet SelectedPacket
        {
            get { return _selectedPacket; }
            set
            {
                _selectedPacket = value;
                OnPropertyChanged();
            }
        }

        private int _selectedPacketTypeIndex;
        public int SelectedPacketTypeIndex
        {
            get { return _selectedPacketTypeIndex; }
            set
            {
                _selectedPacketTypeIndex = value;
                OnPropertyChanged();
            }
        }

        private int _filterOpCode;
        public string FilterOpCode
        {
            get { return _filterOpCode.ToString(); }
            set
            {
                int opCode;
                if(int.TryParse(value, out opCode))
                {
                    _filterOpCode = opCode;
                }
            }
        }

        public MainViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                try
                {

                    AllocConsole();

                    using (_client = new Client(string.Empty))
                    {
                        _client.Logger.Level = Logger.LogLevel.Error;
                        _client.Logger.Output = Logger.LogOutput.Console;

                        _client.Connection.OnReceivedClientMessage += Proxy_OnReceivedClientMessage;
                        _client.Connection.OnReceivedServerMessage += Proxy_OnReceivedServerMessage;

                        _client.Connection.IsClientPacketParsingEnabled = false;
                        _client.Connection.IsServerPacketParsingEnabled = false;
                        _client.StartConnection(httpPort: 7171, loginWebService: string.Empty);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void Proxy_OnReceivedClientMessage(byte[] data)
        {
            AddToPackets(PacketType.Client, data);
        }

        private void Proxy_OnReceivedServerMessage(byte[] data)
        {
            AddToPackets(PacketType.Server, data);
        }

        public void AddToPackets(PacketType type, byte[] data)
        {
            var packet = new Packet(type, data);
            Packets.Add(packet);

            var filterType = (PacketType)(SelectedPacketTypeIndex - 1);
            if (filterType == type)
                if (_filterOpCode > 0)
                {
                    if (_filterOpCode == packet.OpCode)
                        FilteredPackets.Add(packet);
                }
                else
                {
                    FilteredPackets.Add(packet);
                }
        }

        public ICommand FilterCommand => new Command(_ => Filter());
        private void Filter()
        {
            if (Packets == null || Packets.Count < 1)
                return;

            //Filter Type
            IEnumerable<Packet> filteredList = null;
            if (FilteredPackets == null)
                FilteredPackets = new ObservableCollection<Packet>();
            FilteredPackets.Clear();
            if (SelectedPacketTypeIndex == 1)
            {
                filteredList = from packet in Packets where packet.Type == PacketType.Client select packet;
            }
            else if (SelectedPacketTypeIndex == 2)
            {
                filteredList = from packet in Packets where packet.Type == PacketType.Server select packet;
            }
            else
            {
                filteredList = from packet in Packets select packet;
            }

            //Filter OpCode
            if (_filterOpCode > 0)
            {
                filteredList = from packet in filteredList where packet.OpCode == _filterOpCode select packet;
            }

            FilteredPackets = new ObservableCollection<Packet>(filteredList);
        }
    }
}
