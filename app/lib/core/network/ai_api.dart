import 'api_client.dart';
import '../models/ai_models.dart';

class AiApi {
  final ApiClient _client;
  AiApi(this._client);

  Future<AiResponse> ask(String prompt) => _client.guard(() async {
        final response = await _client.dio.post('/api/v1/ai/assistant/ask', data: {'prompt': prompt});
        return AiResponse.fromJson(response.data as Map<String, dynamic>);
      });

  Future<List<AiHistoryItem>> history() => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/ai/assistant/history');
        return (response.data as List<dynamic>).map((e) => AiHistoryItem.fromJson(e as Map<String, dynamic>)).toList();
      });
}
