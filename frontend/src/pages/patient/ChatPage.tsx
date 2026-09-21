import { useEffect, useRef, useState, type FormEvent } from "react";
import { chat } from "../../api/patient";
import { Button } from "../../components/ui/Button";
import { Card } from "../../components/ui/Card";
import { PageHeader } from "../../components/ui/PageHeader";
import { ChatIcon, SendIcon } from "../../components/ui/icons";
import { ErrorList } from "../../components/ui/ErrorList";
import { formatError } from "../../lib/formatError";

interface Message {
  role: "user" | "assistant";
  text: string;
}

function TypingDots() {
  return (
    <div className="flex w-fit items-center gap-2 self-start">
      <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-(--accent-bg) text-(--accent)">
        <ChatIcon className="h-3.5 w-3.5" />
      </span>
      <div className="flex gap-1 rounded-lg bg-(--accent-bg) px-3 py-2.5">
        {[0, 1, 2].map((i) => (
          <span
            key={i}
            className="h-1.5 w-1.5 animate-bounce rounded-full bg-(--accent)"
            style={{ animationDelay: `${i * 0.12}s` }}
          />
        ))}
      </div>
    </div>
  );
}

export default function ChatPage() {
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState("");
  const [sending, setSending] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);
  const scrollRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  useEffect(() => {
    scrollRef.current?.scrollTo({ top: scrollRef.current.scrollHeight, behavior: "smooth" });
  }, [messages, sending]);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (!input.trim() || sending) return;
    setErrors([]);
    const userMessage = input.trim();
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
      inputRef.current?.focus();
    }
  }

  return (
    <div className="mx-auto flex h-[calc(100vh-9rem)] max-w-2xl flex-col gap-4">
      <PageHeader icon={ChatIcon} title="Chat" description="Ask about your doctors, appointments, or medical records." />
      <Card className="flex-1 overflow-hidden p-0">
        <div ref={scrollRef} className="flex h-full flex-col gap-3 overflow-y-auto p-4">
          {messages.length === 0 && (
            <div className="flex h-full flex-col items-center justify-center gap-2 text-center">
              <span className="flex h-10 w-10 items-center justify-center rounded-full bg-(--accent-bg) text-(--accent)">
                <ChatIcon className="h-5 w-5" />
              </span>
              <p className="max-w-xs text-sm text-(--text)">Ask about your doctors, appointments, or medical records.</p>
            </div>
          )}
          {messages.map((m, i) => (
            <div key={i} className={`flex items-end gap-2 ${m.role === "user" ? "self-end flex-row-reverse" : "self-start"}`}>
              {m.role === "assistant" && (
                <span className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full bg-(--accent-bg) text-(--accent)">
                  <ChatIcon className="h-3.5 w-3.5" />
                </span>
              )}
              <div
                className={`max-w-[80%] whitespace-pre-wrap rounded-lg px-3 py-2 text-sm ${
                  m.role === "user" ? "bg-(--accent) text-white" : "bg-(--accent-bg) text-(--text-h)"
                }`}
                style={{ animation: "fade-in 0.15s ease-out" }}
              >
                {m.text}
              </div>
            </div>
          ))}
          {sending && <TypingDots />}
        </div>
      </Card>
      {errors.length > 0 && <ErrorList errors={errors} />}
      <form onSubmit={handleSubmit} className="flex gap-2">
        <input
          ref={inputRef}
          value={input}
          maxLength={2000}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Type a message…"
          disabled={sending}
          className="flex-1 rounded-(--radius) border border-(--border) bg-(--surface) px-3 py-2 text-sm text-(--text-h) outline-none transition-colors focus:border-(--accent) focus:ring-2 focus:ring-(--accent-bg) disabled:opacity-60"
        />
        <Button type="submit" disabled={sending || !input.trim()}>
          <SendIcon className="h-4 w-4" />
          {sending ? "Sending…" : "Send"}
        </Button>
      </form>
    </div>
  );
}
