﻿using System;
using System.IO;

namespace FluentFTP.GnuTLS {

	internal partial class GnuTlsStream : Stream, IDisposable {

		private void DisableNagle() {
			socket.NoDelay = true;
		}

		private void ReEnableNagle() {
			socket.NoDelay = false;
		}

	}
}
