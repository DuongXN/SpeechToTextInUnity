1. Để sử tính năng Speech Recognition (speech to text) hoặc Translation (text to text)
cần được cung cấp API key.
- Tìm tới prefab GC Translate Controllers > GC Translate Controllers và add API key cho Speech Recognition
- Tìm tới prefab GC Translate Controllers > GCTranslation (Text2Text) và add API key cho Text to Text
-> Save Prefab
2. Play Scene VoiceChat để test.
3. Tìm script SpeechToTextHandle.cs để biết cách sử dụng truy cập tới events của GCSpeechRecognition hoặc GCTranslation