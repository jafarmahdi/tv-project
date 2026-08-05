import 'api_client.dart';
import '../models/lists_models.dart';

class ListsApi {
  final ApiClient _client;
  ListsApi(this._client);

  Future<List<UserList>> getMyLists() => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/lists');
        return (response.data as List<dynamic>).map((e) => UserList.fromJson(e as Map<String, dynamic>)).toList();
      });

  Future<List<ListItem>> getItems(String listId) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/lists/$listId/items');
        return (response.data as List<dynamic>).map((e) => ListItem.fromJson(e as Map<String, dynamic>)).toList();
      });

  Future<UserList> createCustomList(String name, {bool isPublic = true}) => _client.guard(() async {
        final response = await _client.dio.post('/api/v1/lists', data: {'name': name, 'isPublic': isPublic});
        return UserList.fromJson(response.data as Map<String, dynamic>);
      });

  Future<void> deleteList(String listId) => _client.guard(() => _client.dio.delete('/api/v1/lists/$listId'));

  Future<void> addItem(String listId, {int? movieTmdbId, int? seriesTmdbId}) => _client.guard(() => _client.dio.post(
        '/api/v1/lists/$listId/items',
        data: {if (movieTmdbId != null) 'movieTmdbId': movieTmdbId, if (seriesTmdbId != null) 'seriesTmdbId': seriesTmdbId},
      ));

  Future<void> removeItem(String listId, String itemId) =>
      _client.guard(() => _client.dio.delete('/api/v1/lists/$listId/items/$itemId'));
}
