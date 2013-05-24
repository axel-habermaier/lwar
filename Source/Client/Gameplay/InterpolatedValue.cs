//using System;

//namespace Lwar.Client.Gameplay
//{
//	using Pegasus.Framework;

//	public struct InterpolatedValue
//	{
//		private Data _data0, _data1, _data2;
//		private State _state;
//		public float CurrentValue { get; private set; }

//		public void AddValue(float value, uint timestamp)
//		{
//			switch (_state)
//			{
//				case State.Interpolating:
//					break;
//				case State.Extrapolating:
//					break;
//				case State.WaitingForData:
//					break;
//			}
//		}

//		public void Update(uint timestamp)
//		{
//			switch (_state)
//			{
//				case State.Interpolating:
//					// If we're out of data, start extrapolating
//					if (timestamp >= _data2.Timestamp)
//					{
//						_state = State.Extrapolating;
//						goto case State.Extrapolating;
//					}

//					// Interpolate between the appropriate data pair
//					if (timestamp >= _data1.Timestamp)
//					{
//						//CurrentValue = Lerp(_data1, _data2, timestamp);
//						break;
//					}

//					//CurrentValue = Lerp(_data0, _data1, timestamp);
//					Assert.That(timestamp >= _data0.Timestamp, "");
//					break;
//				case State.Extrapolating:
//					break;
//				case State.WaitingForData:
//					break;
//			}
//		}

//		private struct Data
//		{
//			public float Timestamp;
//			public float Value;
//		}

//		private enum State
//		{
//			WaitingForData = 0,
//			Interpolating,
//			Extrapolating
//		}
//	}
//}