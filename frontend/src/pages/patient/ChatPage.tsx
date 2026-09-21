import { useState, type FormEvent } from "react";
import { chat } from "../../api/patient";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

interface Message {
  role: "user" | "assistant";
  text: string;
}

export default function ChatPage() {
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState("");
  const [sending, setSending] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (!input.trim()) return;
    setErrors([]);
    const userMessage = input;
    setMessages((m) => [...m, { role: "user", text: userMessage }]);
    setInput("");
    setSending(true);
    try {
      const res = await chat({ message: userMessage });
      setMessages((m) => [...m, { role: "assistant", text: res.reply }]);
    } catch (err) {
      setErrors(formatError(err));
    } finally {
      setSending(false);
    }
  }

  return (
    <div className="mx-auto flex h-[calc(100vh-8rem)] max-w-2xl flex-col gap-4">
      <h2 className="text-lg font-semibold text-(--text-h)">Chat</h2>
      <Card className="flex-1 overflow-y-auto">
        <div className="flex flex-col gap-3">
          {messages.length === 0 && <p className="text-sm text-(--text)">Ask about your doctors or medical records.</p>}
          {messages.map((m, i) => (
            <div
              key={i}
              className={`max-w-[80%] rounded-lg px-3 py-2 text-sm ${
                m.role === "user" ? "self-end bg-(--accent) text-white" : "self-start bg-(--accent-bg) text-(--text-h)"
              }`}
            >
              {m.text}
            </div>
          ))}
        </div>
      </Card>
      {errors.length > 0 && <ErrorList errors={errors} />}
      <form onSubmit={handleSubmit} className="flex gap-2">
        <input
          value={input}
          maxLength={2000}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Type a message…"
          className="flex-1 rounded-md border border-(--border) px-3 py-2 text-(--text-h) outline-none focus:border-(--accent)"
        />
        <Button type="submit" disabled={sending}>
          {sending ? "Sending…" : "Send"}
        </Button>
      </form>
    </div>
  );
}
