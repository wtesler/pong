using System;

public class Message {
	
	public Message (string endpointId, byte[] content, bool isReliable) {
		this.endpointId = endpointId;
		this.content = content;
		this.isReliable = isReliable;
	}

	public string endpointId { get; set; }

	public byte[] content { get; set; }

	public bool isReliable { get; set; }
}

