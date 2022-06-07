using System;
using System.Globalization;
using System.Runtime.InteropServices;
using App.Core.Units;

namespace Sophie.Units
{
	public static class DateTimes
	{
		//public static TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("UTC");
		//public static TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

		public static DateTime Now()
		{
			DateTime utc = DateTime.UtcNow;
			DateTime vietNam = utc.ToVietNam();
			//vietNam = DateTime.SpecifyKind(vietNam, DateTimeKind.Unspecified);
			vietNam = new DateTime(vietNam.Year, vietNam.Month, vietNam.Day,
								   vietNam.Hour, vietNam.Minute, vietNam.Second, DateTimeKind.Unspecified);
            return vietNam;
        }

		public static bool EqualsDate(this DateTime dateTime1, DateTime? dateTime2)
        {
			string _dateTime1 = dateTime1.ToString(format());//"yyyy-MM-dd"
			string _dateTime2 = dateTime2?.ToString(format());//"yyyy-MM-dd"
			return _dateTime1.Equals(_dateTime2);
		}

		public static bool EqualsTime(this DateTime dateTime1, DateTime? dateTime2)
		{
			string _dateTime1 = dateTime1.ToString(format17());//"hh:mm"
			string _dateTime2 = dateTime2?.ToString(format17());//"hh:mm"
			return _dateTime1.Equals(_dateTime2);
		}

		// Cố gắng chuyển đổi string --> DateTime với định dạng bất kỳ, ngoại lệ trả về ngày mặc định
		public static DateTime Parse(string dateString)
		{
			try
			{
				DateTime datetime = DateTime.Parse(dateString);
				//datetime = DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
				//datetime = TimeZoneInfo.ConvertTimeToUtc(datetime, timeZoneInfo);
				return datetime;
			}
			catch (Exception ex)
			{
				Logs.debug($"Exception: {dateString} " + ex.StackTrace);
				DateTime datetime = DateTime.ParseExact("1900-01-01T00:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
				return datetime;
			}
		}

		// Cố gắng chuyển đổi string --> DateTime với định dạng được chỉ định, ngoại lệ trả về ngày mặc định nếu pattern không đúng định dạng
		public static DateTime ParseExact(string dateString, string pattern = "yyyy-MM-ddTHH:mm:ss")
		{
			try
			{
				DateTime datetime = DateTime.ParseExact($"{dateString}", $"{pattern}", CultureInfo.InvariantCulture);
				//datetime = DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
				//datetime = TimeZoneInfo.ConvertTimeToUtc(datetime, timeZoneInfo);
				return datetime;
			}
			catch (Exception ex)
			{
				Logs.debug($"Exception: {dateString} " + ex.StackTrace);
				DateTime datetime = DateTime.ParseExact("1900-01-01T00:00:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
				return datetime;
			}
		}

		public static string ToVietNam(this DateTime datetime, string pattern = "dd/MM/yyyy HH:mm:sszzz")
		{
			if (UntilsEx.GetSystemType() == OSPlatform.OSX || UntilsEx.GetSystemType() == OSPlatform.Linux)
				return TimeZoneInfo.ConvertTime(datetime, TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh")).ToString(pattern);
			else
				return TimeZoneInfo.ConvertTime(datetime, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).ToString(pattern);
		}

		public static DateTime ToVietNam(this DateTime datetime)
		{
			if (UntilsEx.GetSystemType() == OSPlatform.OSX || UntilsEx.GetSystemType() == OSPlatform.Linux)
				return TimeZoneInfo.ConvertTime(datetime, TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"));
			else
				return TimeZoneInfo.ConvertTime(datetime, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
		}

		// 2020-06-15
		public static string format()
		{
			return "yyyy-MM-dd";
		}

		// 15 July 2020
		public static string format1()
		{
			return "dd MMMM yyyy";
		}

		// 2020-06-15T10:31:00+11:00
		public static string format2()
		{
			return "yyyy-MM-ddTHH:mm:sszzz";
		}

		// 2020-06-15 10:31
		public static string format3()
		{
			return "yyyy-MM-dd HH:mm";
		}

		// 202006151031
		public static string format4()
		{
			return "yyyyMMddHHmm";
		}

		// 08/26/2020, 02:27 PM
		public static string format5()
		{
			return "MM/dd/yyyy hh:mm tt";
		}

		// 08/26/2020, 23:27 PM
		public static string format6()
		{
			return "MM/dd/yyyy HH:mm tt";
		}

		// 23:27 PM
		public static string format7()
		{
			return "HH:mm tt";
		}

		//Mar 31, 2020, 2:02 PM
		public static string format8()
		{
			return "MMM dd, yyyy, HH:mm tt";
		}

		// 15 Jul 2020
		public static string format9()
		{
			return "dd MMM yyyy";
		}

		// 15/06/2020
		public static string format10()
		{
			return "dd/MM/yyyy";
		}

		// 2021-11-01T13:00:00+07:00
		public static string format11()
		{
			return "yyyy-MM-ddTHH:mm:sszzz";
		}

		// 06:30:00+07:00
		public static string format12()
		{
			return "hh:mm:sszzz";
		}

		// 18:30:00+07:00
		public static string format13()
		{
			return "HH:mm:sszzz";
		}

		// 15/06/2020 18:30:00+07:00
		public static string format14()
		{
			return "dd/MM/yyyy HH:mm:sszzz";
		}

		// 2021-11-01T13:00:00Z
		public static string format15()
		{
			return "yyyy-MM-ddTHH:mm:ssZ";
		}

		// 2021-11-01T13:00:00
		public static string format16()
		{
			return "yyyy-MM-ddTHH:mm:ss";
		}

		// 18:30
		public static string format17()
		{
			return "HH:mm";
		}


		#region Các hàm chuyển đổi
		/* 
		 * Chuyển đổi dương lịch --> âm lịch, solar calendar to lunar calendar
		 */
		public static LunarDate SolarToLunar(DateTime date)
		{
			return SolarToLunar(date, 7);
		}

		public static LunarDate SolarToLunar(DateTime date, int timeZone)
		{
			long k, dayNumber, monthStart, a11, b11, lunarDay, lunarMonth, lunarYear, diff, leapMonthDiff;
			bool lunarLeap;

			dayNumber = jdFromDate(date.Day, date.Month, date.Year);
			k = INT((dayNumber - 2415021.076998695) / 29.530588853);
			monthStart = getNewMoonDay(k + 1, timeZone);
			if (monthStart > dayNumber)
			{
				monthStart = getNewMoonDay(k, timeZone);
			}
			// alert(dayNumber+" -> "+monthStart);
			a11 = getLunarMonth11(date.Year, timeZone);
			b11 = a11;
			if (a11 >= monthStart)
			{
				lunarYear = date.Year;
				a11 = getLunarMonth11(date.Year - 1, timeZone);
			}
			else
			{
				lunarYear = date.Year + 1;
				b11 = getLunarMonth11(date.Year + 1, timeZone);
			}
			lunarDay = dayNumber - monthStart + 1;
			diff = INT((monthStart - a11) / 29);
			lunarLeap = false;
			lunarMonth = diff + 11;
			if (b11 - a11 > 365)
			{
				leapMonthDiff = getLeapMonthOffset(a11, timeZone);
				if (diff >= leapMonthDiff)
				{
					lunarMonth = diff + 10;
					if (diff == leapMonthDiff)
					{
						lunarLeap = true;
					}
				}
			}
			if (lunarMonth > 12)
			{
				lunarMonth = lunarMonth - 12;
			}
			if (lunarMonth >= 11 && diff < 4)
			{
				lunarYear -= 1;
			}
			return new LunarDate((int)lunarDay, (int)lunarMonth, (int)lunarYear, lunarLeap);
		}

		/* 
		 * Chuyển đổi âm lịch --> dương lịch, lunar calendar to solar calendar
		 */
		public static DateTime LunarToSolar(LunarDate ld)
		{
			return LunarToSolar(ld, 7);
		}

		public static DateTime LunarToSolar(LunarDate ld, int timeZone)
		{
			long k, a11, b11, off, leapOff, leapMonth, monthStart;
			if (ld.Month < 11)
			{
				a11 = getLunarMonth11(ld.Year - 1, timeZone);
				b11 = getLunarMonth11(ld.Year, timeZone);
			}
			else
			{
				a11 = getLunarMonth11(ld.Year, timeZone);
				b11 = getLunarMonth11(ld.Year + 1, timeZone);
			}
			k = INT(0.5 + (a11 - 2415021.076998695) / 29.530588853);
			off = ld.Month - 11;
			if (off < 0)
			{
				off += 12;
			}
			if (b11 - a11 > 365)
			{
				leapOff = getLeapMonthOffset(a11, timeZone);
				leapMonth = leapOff - 2;
				if (leapMonth < 0)
				{
					leapMonth += 12;
				}
				if (ld.IsLeapYear && ld.Month != leapMonth)
				{
					return DateTime.MinValue;
				}
				else if (ld.IsLeapYear || off >= leapOff)
				{
					off += 1;
				}
			}
			monthStart = getNewMoonDay(k + off, timeZone);
			return jdToDate(monthStart + ld.Day - 1);
		}
		#endregion Các hàm chuyển đổi
			

		#region Các hàm tính toán chung
		private const double PI = Math.PI;

		/* 
		 * Discard the fractional part of a number, e.g., INT(3.2) = 3 
		 */
		public static long INT(double d)
		{
			return (long)Math.Floor(d);
		}

		/* 
		 * Compute the (integral) Julian day number of day dd/mm/yyyy, i.e., the number
		 * of days between 1/1/4713 BC (Julian calendar) and dd/mm/yyyy.
		 * Formula from http://www.tondering.dk/claus/calendar.html
		 */
		private static long jdFromDate(int dd, int mm, int yy)
		{
			long a = INT((14 - mm) / 12);
			long y = yy + 4800 - a;
			long m = mm + 12 * a - 3;
			long jd = dd + INT((153 * m + 2) / 5) + 365 * y + INT(y / 4) - INT(y / 100) + INT(y / 400) - 32045;
			if (jd < 2299161)
			{
				jd = dd + INT((153 * m + 2) / 5) + 365 * y + INT(y / 4) - 32083;
			}
			return jd;
		}

		/* 
		 * Convert a Julian day number to day/month/year. 
		 * Parameter jd is an integer 
		 */
		public static DateTime jdToDate(long jd)
		{
			long a, b, c, d, e, m, day, month, year;
			if (jd > 2299160)
			{ // After 5/10/1582, Gregorian calendar
				a = jd + 32044;
				b = INT((4 * a + 3) / 146097);
				c = a - INT((b * 146097) / 4);
			}
			else
			{
				b = 0;
				c = jd + 32082;
			}
			d = INT((4 * c + 3) / 1461);
			e = c - INT((1461 * d) / 4);
			m = INT((5 * e + 2) / 153);
			day = e - INT((153 * m + 2) / 5) + 1;
			month = m + 3 - 12 * INT(m / 10);
			year = b * 100 + d - 4800 + INT(m / 10);
			return new DateTime((int)year, (int)month, (int)day);
		}

		/* 
		 * Compute the time of the k-th new moon after the new moon of 1/1/1900 13:52 UCT
		 * (measured as the number of days since 1/1/4713 BC noon UCT, e.g., 2451545.125 is 1/1/2000 15:00 UTC).
		 * Returns a floating number, e.g., 2415079.9758617813 for k=2 or 2414961.935157746 for k=-2
		 * Algorithm from: "Astronomical Algorithms" by Jean Meeus, 1998
		 */
		public static long NewMoon(long k)
		{
			double T, T2, T3, dr, Jd1, M, Mpr, F, C1, deltat, JdNew;
			T = k / 1236.85; // Time in Julian centuries from 1900 January 0.5
			T2 = T * T;
			T3 = T2 * T;
			dr = PI / 180;
			Jd1 = 2415020.75933 + 29.53058868 * k + 0.0001178 * T2 - 0.000000155 * T3;
			Jd1 = Jd1 + 0.00033 * Math.Sin((166.56 + 132.87 * T - 0.009173 * T2) * dr); // Mean new moon
			M = 359.2242 + 29.10535608 * k - 0.0000333 * T2 - 0.00000347 * T3; // Sun's mean anomaly
			Mpr = 306.0253 + 385.81691806 * k + 0.0107306 * T2 + 0.00001236 * T3; // Moon's mean anomaly
			F = 21.2964 + 390.67050646 * k - 0.0016528 * T2 - 0.00000239 * T3; // Moon's argument of latitude
			C1 = (0.1734 - 0.000393 * T) * Math.Sin(M * dr) + 0.0021 * Math.Sin(2 * dr * M);
			C1 = C1 - 0.4068 * Math.Sin(Mpr * dr) + 0.0161 * Math.Sin(dr * 2 * Mpr);
			C1 = C1 - 0.0004 * Math.Sin(dr * 3 * Mpr);
			C1 = C1 + 0.0104 * Math.Sin(dr * 2 * F) - 0.0051 * Math.Sin(dr * (M + Mpr));
			C1 = C1 - 0.0074 * Math.Sin(dr * (M - Mpr)) + 0.0004 * Math.Sin(dr * (2 * F + M));
			C1 = C1 - 0.0004 * Math.Sin(dr * (2 * F - M)) - 0.0006 * Math.Sin(dr * (2 * F + Mpr));
			C1 = C1 + 0.0010 * Math.Sin(dr * (2 * F - Mpr)) + 0.0005 * Math.Sin(dr * (2 * Mpr + M));
			if (T < -11)
			{
				deltat = 0.001 + 0.000839 * T + 0.0002261 * T2 - 0.00000845 * T3 - 0.000000081 * T * T3;
			}
			else
			{
				deltat = -0.000278 + 0.000265 * T + 0.000262 * T2;
			};
			JdNew = Jd1 + C1 - deltat;
			return (long)Math.Round(JdNew);
		}

		/* 
		 * Compute the longitude of the sun at any time.
		 * Parameter: floating number jdn, the number of days since 1/1/4713 BC noon
		 * Algorithm from: "Astronomical Algorithms" by Jean Meeus, 1998
		 */
		public static double SunLongitude(double jdn)
		{
			double T, T2, dr, M, L0, DL, L;
			T = (jdn - 2451545.0) / 36525; // Time in Julian centuries from 2000-01-01 12:00:00 GMT
			T2 = T * T;
			dr = PI / 180; // degree to radian
			M = 357.52910 + 35999.05030 * T - 0.0001559 * T2 - 0.00000048 * T * T2; // mean anomaly, degree
			L0 = 280.46645 + 36000.76983 * T + 0.0003032 * T2; // mean longitude, degree
			DL = (1.914600 - 0.004817 * T - 0.000014 * T2) * Math.Sin(dr * M);
			DL = DL + (0.019993 - 0.000101 * T) * Math.Sin(dr * 2 * M) + 0.000290 * Math.Sin(dr * 3 * M);
			L = L0 + DL; // true longitude, degree
			L = L * dr;
			L = L - PI * 2 * (INT(L / (PI * 2))); // Normalize to (0, 2*PI)
			return L;
		}

		/* 
		 * Compute sun position at midnight of the day with the given Julian day number.
		 * The time zone if the time difference between local time and UTC: 7.0 for UTC+7:00.
		 * The function returns a number between 0 and 11.
		 * From the day after March equinox and the 1st major term after March equinox, 0 is returned.
		 * After that, return 1, 2, 3 ...
		 */
		public static long getSunLongitude(long dayNumber, int timeZone)
		{
			return INT(SunLongitude(dayNumber - 0.5 - timeZone / 24) / PI * 6);
		}

		/* 
		 * Compute the day of the k-th new moon in the given time zone.
		 * The time zone if the time difference between local time and UTC: 7.0 for UTC+7:00
		 */
		public static long getNewMoonDay(long k, int timeZone)
		{
			return INT(NewMoon(k) + 0.5 + timeZone / 24);
		}

		/* 
		 * Find the day that starts the luner month 11 of the given year for the given time zone 
		 */
		public static long getLunarMonth11(int yy, int timeZone)
		{
			long k, off, nm, sunLong;
			//off = jdFromDate(31, 12, yy) - 2415021.076998695;
			off = jdFromDate(31, 12, yy) - 2415021;
			k = INT(off / 29.530588853);
			nm = getNewMoonDay(k, timeZone);
			sunLong = getSunLongitude(nm, timeZone); // sun longitude at local midnight
			if (sunLong >= 9)
			{
				nm = getNewMoonDay(k - 1, timeZone);
			}
			return nm;
		}

		/* 
		 * Find the index of the leap month after the month starting on the day a11. 
		 */
		public static long getLeapMonthOffset(long a11, int timeZone)
		{
			long k, last, arc, i;
			k = INT((a11 - 2415021.076998695) / 29.530588853 + 0.5);
			last = 0;
			i = 1; // We start with the month following lunar month 11
			arc = getSunLongitude(getNewMoonDay(k + i, timeZone), timeZone);
			do
			{
				last = arc;
				i++;
				arc = getSunLongitude(getNewMoonDay(k + i, timeZone), timeZone);
			} while (arc != last && i < 14);
			return i - 1;
		}
		#endregion Các hàm tính toán chung


		public static bool checkLeapYear(int year)
		{
			// Nếu số năm chia hết cho 400, đó là 1 năm nhuận
			if (year % 400 == 0)
				return true;

			// Nếu số năm chia hết cho 4 và không chia hết cho 100, đó không là 1 năm nhuận
			if (year % 4 == 0 && year % 100 != 0)
				return true;

			// trường hợp còn lại không phải năm nhuận
			return false;
		}

		public class LunarDate
		{
			public int Day { get; set; }
			public int Month { get; set; }
			public int Year { get; set; }
			public bool IsLeapYear { get; set; }//Là năm nhuận

			public LunarDate()
			{
			}

			public LunarDate(int day, int month, int year, bool leap)
			{
				Day = day;
				Month = month;
				Year = year;
				IsLeapYear = leap;
			}

			public override string ToString()
			{
				return Day.ToString() + "/" + Month.ToString() + "/" + Year.ToString() + (IsLeapYear ? "-Nhuận" : "-Không Nhuận");
			}

			public DateTime ToSolarDate(int timeZone)
			{
				return LunarToSolar(this);
			}

			public DateTime ToSolarDate()
			{
				return ToSolarDate(7);
			}
		}

	}
}
