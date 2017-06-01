﻿using System;
using System.Globalization;
using System.Text;

namespace DashboardServer
{
	public class ExchangeData
	{
		public ExchangeData()
		{
			LastTimeInPit = -1;
			LastTimeOnTrack = -1;
		}

		public enum FlagIndex
		{
			NO_FLAG = -1,
			GREEN = 0,
			YELLOW = 1,
			BLUE = 2,
			BLACK = 3,
			BLACK_WHITE = 4,
			WHITE = 5,
			CHECKERED = 6,
			PENALTY = 7
		}

		public enum RaceFormatIndex
		{
			NOT_AVAILABLE = -1,
			TIME = 0,
			LAP = 1,
			TIME_AND_EXTRA_LAP = 2
		}

		public enum SessionIndex
		{
			UNKNOWN = -1,
			PRACTICE = 0,
			QUALIFY = 1,
			RACE = 2
		}

		// Internal.
		private double _fuelLeftLastLap = -1;

		// Basic data.
		public int CarSpeed { get; internal set; }
		public int Gear { get; internal set; }
		public int MaxEngineRpm { get; internal set; }
		public int EngineRpm { get; internal set; }

		// DRS.
		public int DrsEquipped { get; internal set; }
		public int DrsAvailable { get; internal set; }
		public int DrsEngaged { get; internal set; }
		public int DrsNumActivationsLeft { get; internal set; }

		// ERS.
		public int ErsEquipped { get; internal set; }

		// KERS.
		public int KersEquipped { get; internal set; }

		// P2P.
		public int PushToPassEquipped { get; internal set; }
		public int PushToPassAvailable { get; internal set; }
		public int PushToPassEngaged { get; internal set; }
		public int PushToPassNumActivationsLeft { get; internal set; }
		public double PushToPassEngagedTimeLeft { get; internal set; }
		public double PushToPassWaitTimeLeft { get; internal set; }

		// Fuel.
		public double FuelLapsLeftEstimate
		{
			get
			{
				if (FuelPerLap > 0)
				{
					return LapsUntilSessionEnd * FuelPerLap;
				}

				return -1;
			}
		}

		public double LapsUntilSessionEnd
        {
            get
            {
                double lapFraction = 0.0;
                double lapsToGo = 0.0;
                if (Session == (int)SessionIndex.PRACTICE || Session == (int)SessionIndex.QUALIFY ||
                    (Session == (int)SessionIndex.RACE && (RaceFormat == (int)RaceFormatIndex.TIME || RaceFormat == (int)RaceFormatIndex.TIME_AND_EXTRA_LAP)
                    ))
                {
                    double bestLapWithBuffer = LapTimeBestSelf * 1.03; // Add 3% to lap time. It's a bit more secure.
                    double drivenInLap = LapTimeCurrentSelf / (bestLapWithBuffer + DeltaBestSelf);
                    double remainingInLap = 1.0 - drivenInLap;
                    double remainingSecInLap = drivenInLap * bestLapWithBuffer;
                    double remainingSessionTimeAtFinishLine = remainingSecInLap < SessionTimeRemaining ? SessionTimeRemaining - remainingSecInLap : 0;
                    double remainingFullLapsInSession = Math.Ceiling(remainingSessionTimeAtFinishLine / bestLapWithBuffer);

                    lapsToGo = remainingFullLapsInSession + remainingInLap;
                }

                if (Session == (int)SessionIndex.RACE)
                {
                    if (RaceFormat == (int)RaceFormatIndex.TIME_AND_EXTRA_LAP)
                    {
                        lapsToGo++;
                    }
                    else if (RaceFormat == (int)RaceFormatIndex.LAP)
                    {
                        lapsToGo = (NumberOfLaps - CompletedLaps) - lapFraction;
                    }
                    else
                    {
                        lapsToGo = -1;
                    }
                }

                return lapsToGo;
            }
        }

		public double FuelLeft { get; internal set; }
		public double FuelPerLap { get; internal set; }
		public double FuelMax { get; internal set; }

		// Misc.
		public int NumberOfLaps { get; internal set; }
		public int CompletedLaps { get; internal set; }
		public int RaceFormat { get; internal set; }
		public int Session { get; internal set; }
		public int SessionIteration { get; internal set; }

		public int Position { get; internal set; }
		public int NumCars { get; internal set; }
		public int PitLimiter { get; internal set; }
		public int InPitLane { get; internal set; }
		public double DistanceTravelled { get; internal set; }

		// Lap times.
		public double LapTimeCurrentSelf { get; internal set; }
		public double LapTimePreviousSelf { get; internal set; }
		public double LapTimeBestSelf { get; internal set; }
		public double SessionTimeRemaining { get; internal set; }
		public double LapTimeBestLeader { get; internal set; }
		public double LapTimeBestLeaderClass { get; internal set; }

		// Deltas.
		public double DeltaBestSelf { get; internal set; }
		public double DeltaBestSession { get; internal set; }
		public double LapTimeDeltaLeader { get; internal set; }
		public double LapTimeDeltaLeaderClass { get; internal set; }
		public double TimeDeltaBehind { get; internal set; }
		public double TimeDeltaFront { get; internal set; }

		// Sector stuff and flags.
		public int CurrentSector { get; internal set; }

		// Temperatures.
		public double AirTemperature { get; internal set; }
		public double TrackTemperature { get; internal set; }
		public double OilTemperature { get; internal set; }
		public double WaterTemperature { get; internal set; }

		// Tire temperatures.
		public double TireTempFrontLeft { get; internal set; }
		public double TireTempFrontRight { get; internal set; }
		public double TireTempRearLeft { get; internal set; }
		public double TireTempRearRight { get; internal set; }

		// Tire wear.
		public double TireWearFrontLeft { get; internal set; }
		public double TireWearFrontRight { get; internal set; }
		public double TireWearRearLeft { get; internal set; }
		public double TireWearRearRight { get; internal set; }

		// Tire pressure.
		public double TirePressureFrontLeft { get; internal set; }
		public double TirePressureFrontRight { get; internal set; }
		public double TirePressureRearLeft { get; internal set; }
		public double TirePressureRearRight { get; internal set; }

		// Tire dirt.
		public double TireDirtFrontLeft { get; internal set; }
		public double TireDirtFrontRight { get; internal set; }
		public double TireDirtRearLeft { get; internal set; }
		public double TireDirtRearRight { get; internal set; }

		// Flag.
		public bool YellowFlagAhead { get; internal set; }
		public int CurrentFlag { get; internal set; }
		public int YellowSector1 { get; internal set; }
		public int YellowSector2 { get; internal set; }
		public int YellowSector3 { get; internal set; }

		// Timestamps.
		public long LastTimeInPit { get; internal set; }
		public long LastTimeOnTrack { get; internal set; }

		public void TriggerFuelCalculation()
		{
			if (_fuelLeftLastLap > -1 && FuelLeft > -1 && _fuelLeftLastLap > FuelLeft)
			{
				FuelPerLap = Math.Round(_fuelLeftLastLap - FuelLeft, 3);
			}
			else
			{
				FuelPerLap = -1;
			}

			_fuelLeftLastLap = FuelLeft;
		}

		public String ToJSON()
		{
			StringBuilder sb = new StringBuilder();
			String appender = "";

			foreach (var prop in GetType().GetProperties())
			{
				sb.Append(appender);

				String niceValue = "";
				Object value = prop.GetValue(this, null);

				if (value is String)
				{
					niceValue = "\"" + value.ToString() + "\"";
				}
				else if (value is double)
				{
					niceValue = ((double)value).ToString("0.000", CultureInfo.CreateSpecificCulture("EN-us"));
				}
				else if (value is int)
				{
					niceValue = ((int)value).ToString("0", CultureInfo.CreateSpecificCulture("EN-us"));
				}
				else if (value is bool)
				{
					niceValue = (bool)value ? "true" : "false";
				}
				else if (value is long)
				{
					niceValue = ((long)value).ToString("0", CultureInfo.CreateSpecificCulture("EN-us"));
				}

				sb.AppendFormat("\"{0}\":{1}", prop.Name, niceValue);

				appender = ",";
			}

			return String.Format("{{{0}}}", sb.ToString());
		}
	}
}
