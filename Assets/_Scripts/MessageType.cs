using System;

public class MessageType {
	private MessageType() {}

	public static byte[] TIME = FromString("aaaa");
	public static byte[] MENU = FromString("aaab");
	public static byte[] NCC = FromString("aaac");
	public static byte[] COOP = FromString("aaad");

	private static byte[] FromString(string str) {
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}
}

