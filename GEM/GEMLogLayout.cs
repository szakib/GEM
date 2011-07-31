using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Layout;
using System.IO;
using log4net.Core;

namespace GEM
{
    class GEMLogLayout : LayoutSkeleton
    {
        /// <summary>
        /// Length of the non-message part of the formatting
        /// </summary>
        public const int padding = 23;

		#region Constructors

		/// <summary>
		/// Constructs a SimpleLayout
		/// </summary>
        public GEMLogLayout()
		{
			IgnoresException = true;
		}

		#endregion
  
		#region Implementation of IOptionHandler

		/// <summary>
		/// Initialize layout options
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is part of the <see cref="IOptionHandler"/> delayed object
		/// activation scheme. The <see cref="ActivateOptions"/> method must 
		/// be called on this object after the configuration properties have
		/// been set. Until <see cref="ActivateOptions"/> is called this
		/// object is in an undefined state and must not be used. 
		/// </para>
		/// <para>
		/// If any of the configuration properties are modified then 
		/// <see cref="ActivateOptions"/> must be called again.
		/// </para>
		/// </remarks>
		override public void ActivateOptions() 
		{
			// nothing to do.
		}

		#endregion

		#region Override implementation of LayoutSkeleton

		/// <summary>
		/// Produces a simple formatted output.
		/// </summary>
		/// <param name="loggingEvent">the event being logged</param>
		/// <param name="writer">The TextWriter to write the formatted event to</param>
		/// <remarks>
		/// <para>
		/// Formats the event as the level of the even,
		/// followed by " - " and then the log message itself. The
		/// output is terminated by a newline.
		/// </para>
		/// </remarks>
		override public void Format(TextWriter writer, LoggingEvent loggingEvent) 
		{
			if (loggingEvent == null)
			{
				throw new ArgumentNullException("loggingEvent");
			}

            writer.Write(DateTime.Now.ToString().PadRight(padding));
			//writer.Write(loggingEvent.Level.DisplayName);
			//writer.Write(" - ");
			loggingEvent.WriteRenderedMessage(writer);
			writer.WriteLine();
		}

		#endregion
    }
}
