using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.ComponentModel;
using System.Text;

namespace NxGen.EmailService.BizComp.Credentials
{
	public class EncryptionHelper : System.IDisposable
	{
		# region Constants
		private byte[] key = { 1, 2, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 22, 27, 28, 51, 30, 31, 87, 33, 34 };
		private byte[] iv = { 19, 18, 17, 16, 15, 14, 13, 12 };
		# endregion
		# region Variables
		private bool disposed = false;
		private IntPtr handle;
		private Component component = new Component();
		private TripleDESCryptoServiceProvider cryptdes = new TripleDESCryptoServiceProvider();
		private UTF8Encoding utf8 = new UTF8Encoding();
		# endregion
		# region Public Methods
		public string EncrypToText(string textToEncrypt)
		{
			try
			{
				return Convert.ToBase64String(this.EncryptToBinary(textToEncrypt));
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public byte[] EncryptToBinary(string textToEncrypt)
		{
			try
			{
				byte[] input = this.utf8.GetBytes(textToEncrypt);
				return this.Transform(input, this.cryptdes.CreateEncryptor(this.key, this.iv));
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public string DecryptToText(string textToDecrypt)
		{
			try
			{
				return this.DecryptToText(Convert.FromBase64String(textToDecrypt));
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public string DecryptToText(byte[] binaryToDecrypt)
		{
			try
			{
				byte[] output = this.Transform(binaryToDecrypt, this.cryptdes.CreateDecryptor(this.key, this.iv));
				return this.utf8.GetString(output);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		# endregion
		# region Private Methods
		private byte[] Transform(byte[] input, ICryptoTransform CryptoTransform)
		{
			MemoryStream memStream = new MemoryStream();
			CryptoStream cryptStream = new CryptoStream(memStream, CryptoTransform, CryptoStreamMode.Write);
			cryptStream.Write(input, 0, input.Length);
			cryptStream.FlushFinalBlock();
			memStream.Position = 0;
			byte[] result = memStream.ToArray();
			memStream.Close();
			cryptStream.Close();
			return result;
		}
		protected virtual void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (!this.disposed)
			{

				if (disposing)
				{
					// Dispose managed resources.
					component.Dispose();
				}
				CloseHandle(handle);
				handle = IntPtr.Zero;
				disposed = true;

			}
		}
		[System.Runtime.InteropServices.DllImport("Kernel32")]
		private extern static Boolean CloseHandle(IntPtr handle);
		~EncryptionHelper()
		{
			Dispose(false);
		}
		# endregion

	}
}