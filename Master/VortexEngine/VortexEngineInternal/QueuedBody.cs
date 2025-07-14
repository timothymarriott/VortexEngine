

using VortexEngine;

public class QueuedBody : Body {

    public QueuedBody(string target) {
        this.target = target;
        this.Name = "QUEUED_BODY";
    }
    public string target;
}

