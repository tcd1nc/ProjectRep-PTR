﻿using System;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace PTR
{
	
	/// <summary>
	/// Provides notification of status changes related to Internet-specific network
	/// adapters on this machine.  All other adpaters such as tunneling and loopbacks
	/// are ignored.  Only connected IP adapters are considered.
	/// </summary>
	/// <remarks>
	/// <i>Implementation Note:</i>
	/// <para>
	/// Since we'll likely invoke the IsAvailable property very frequently, that should
	/// be very efficient.  So we wire up handlers for both NetworkAvailabilityChanged
	/// and NetworkAddressChanged and capture the state in the local isAvailable variable. 
	/// </para>
	/// </remarks>

	public static class NetworkStatus
	{
		private static bool isAvailable;
		private static NetworkStatusChangedHandler handler;

		//========================================================================================
		// Constructor
		//========================================================================================

		/// <summary>
		/// Initialize the class by detecting the start condition.
		/// </summary>

		static NetworkStatus ()
		{
			isAvailable = IsNetworkAvailable();
		}

		//========================================================================================
		// Properties
		//========================================================================================

		/// <summary>
		/// This event is fired when the overall Internet connectivity changes.  All
		/// non-Internet adpaters are ignored.  If at least one valid Internet connection
		/// is "up" then we consider the Internet "available".
		/// </summary>

		public static event NetworkStatusChangedHandler AvailabilityChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				if (handler == null)
				{
					NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(DoNetworkAvailabilityChanged);
                    NetworkChange.NetworkAddressChanged	+= new NetworkAddressChangedEventHandler(DoNetworkAddressChanged);
				}

				handler = (NetworkStatusChangedHandler)Delegate.Combine(handler, value);
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				handler = (NetworkStatusChangedHandler)Delegate.Remove(handler, value);

				if (handler == null)
				{
					NetworkChange.NetworkAvailabilityChanged -= new NetworkAvailabilityChangedEventHandler(DoNetworkAvailabilityChanged);
                    NetworkChange.NetworkAddressChanged	-= new NetworkAddressChangedEventHandler(DoNetworkAddressChanged);
				}
			}
		}


		/// <summary>
		/// Gets a Boolean value indicating the current state of Internet connectivity.
		/// </summary>

		public static bool IsAvailable
		{
			get { return isAvailable; }
		}


		//========================================================================================
		// Methods
		//========================================================================================

		/// <summary>
		/// Evaluate the online network adapters to determine if at least one of them
		/// is capable of connecting to the Internet.
		/// </summary>
		/// <returns></returns>

		private static bool IsNetworkAvailable ()
		{
			// only recognizes changes related to Internet adapters
			if (NetworkInterface.GetIsNetworkAvailable())
			{
				// however, this will include all adapters
				NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
				foreach (NetworkInterface face in interfaces)
				{
					// filter so we see only Internet adapters
					if (face.OperationalStatus == OperationalStatus.Up)
					{
						if ((face.NetworkInterfaceType != NetworkInterfaceType.Tunnel) &&
							(face.NetworkInterfaceType != NetworkInterfaceType.Loopback))
						{
							IPv4InterfaceStatistics statistics = face.GetIPv4Statistics();

							// all testing seems to prove that once an interface comes online
							// it has already accrued statistics for both received and sent...

							if ((statistics.BytesReceived > 0) && (statistics.BytesSent > 0))
							{
                                //if(CanSeeBuckmanConnect())
								    return true;
							}
						}
					}
				}
			}

			return false;
		}


		private static void DoNetworkAddressChanged (object sender, EventArgs e)
		{
			SignalAvailabilityChange(sender);
		}


		private static void DoNetworkAvailabilityChanged (
			object sender, NetworkAvailabilityEventArgs e)
		{
			SignalAvailabilityChange(sender);
		}


		private static void SignalAvailabilityChange (object sender)
		{
			bool change = IsNetworkAvailable();

			if (change != isAvailable)
			{
				isAvailable = change;

				//if (handler != null)
			//	{
					handler?.Invoke(sender, new NetworkStatusChangedArgs(isAvailable));
				//}
			}
		}


        //public static bool CanSeeBuckmanConnect()
        //{
        //    WebClient client = new WebClient();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Application.Current.Resources["BuckmanConnectURL"].ToString()))
        //        {
        //            using (client.OpenRead(Application.Current.Resources["BuckmanConnectURL"].ToString()))
        //            {
        //            }
        //            return true;
        //        }
        //        else
        //            return false;
        //    }
        //    catch (WebException)
        //    {
        //        return false;
        //    }
        //}

    }

    //********************************************************************************************
    // class NetworkStatusChangedArgs
    //********************************************************************************************

    /// <summary>
    /// Describes the overall network connectivity of the machine.
    /// </summary>

    public class NetworkStatusChangedArgs : EventArgs
    {
        private bool isAvailable;

        /// <summary>
        /// Instantiate a new instance with the given availability.
        /// </summary>
        /// <param name="isAvailable"></param>

        public NetworkStatusChangedArgs(bool isAvailable)
        {
            this.isAvailable = isAvailable;
        }

        /// <summary>
        /// Gets a Boolean value indicating the current state of Internet connectivity.
        /// </summary>

        public bool IsAvailable
        {
            get { return isAvailable; }
        }
    }


    //********************************************************************************************
    // delegate NetworkStatusChangedHandler
    //********************************************************************************************

    /// <summary>
    /// Define the method signature for network status changes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    public delegate void NetworkStatusChangedHandler(object sender, NetworkStatusChangedArgs e);
}