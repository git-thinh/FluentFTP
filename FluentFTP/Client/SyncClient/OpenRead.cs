﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using FluentFTP.Helpers;
using System.Threading;
using System.Threading.Tasks;

namespace FluentFTP {
	public partial class FtpClient {

		/// <summary>
		/// Opens the specified file for reading
		/// </summary>
		/// <param name="path">The full or relative path of the file</param>
		/// <param name="type">ASCII/Binary</param>
		/// <param name="restart">Resume location</param>
		/// <param name="checkIfFileExists">Only set this to false if you are SURE that the file does not exist. If true, it reads the file size and saves it into the stream length.</param>
		/// <returns>A stream for reading the file on the server</returns>
		//[Obsolete("OpenRead() is obsolete, please use Download() or DownloadFile() instead", false)]
		public virtual Stream OpenRead(string path, FtpDataType type = FtpDataType.Binary, long restart = 0, bool checkIfFileExists = true) {
			return OpenReadInternal(path, type, restart, checkIfFileExists ? 0 : -1, true);
		}

		/// <summary>
		/// Opens the specified file for reading
		/// </summary>
		/// <param name="path">The full or relative path of the file</param>
		/// <param name="type">ASCII/Binary</param>
		/// <param name="restart">Resume location</param>
		/// <param name="fileLen">
		/// <para>Pass in a file length if known</para>
		/// <br> -1 => File length is irrelevant, do not attempt to determine it</br>
		/// <br> 0  => File length is unknown, try to determine it</br>
		/// <br> >0 => File length is KNOWN. No need to determine it</br>
		/// </param>
		/// <returns>A stream for reading the file on the server</returns>
		//[Obsolete("OpenRead() is obsolete, please use Download() or DownloadFile() instead", false)]
		public virtual Stream OpenRead(string path, FtpDataType type, long restart, long fileLen) {
			return OpenReadInternal(path, type, restart, fileLen, true);
		}

		/// <summary>
		/// Internal routine
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		/// <param name="restart"></param>
		/// <param name="fileLen"></param>
		/// <param name="ignoreStaleData">Normally false. Obsolete API uses true</param>
		/// <returns>A stream for reading the file on the server</returns>
		public virtual Stream OpenReadInternal(string path, FtpDataType type, long restart, long fileLen, bool ignoreStaleData) {
			// verify args
			if (path.IsBlank()) {
				throw new ArgumentException("Required parameter is null or blank.", nameof(path));
			}

			path = path.GetFtpPath();
			LastStreamPath = path;

			LogFunction(nameof(OpenRead), new object[] { path, type, restart, fileLen });

			var client = this;
			FtpDataStream stream = null;
			long length = 0;

			lock (m_lock) {

				length = fileLen == 0 ? client.GetFileSize(path) : fileLen;

				client.SetDataType(type);
				stream = client.OpenDataStream("RETR " + path, restart);
			}

			if (stream != null) {
				if (length > 0) {
					stream.SetLength(length);
				}

				if (restart > 0) {
					stream.SetPosition(restart);
				}
			}

			Status.IgnoreStaleData = ignoreStaleData;

			return stream;
		}

	}
}
