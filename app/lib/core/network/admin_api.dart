import '../models/admin_models.dart';
import 'api_client.dart';

class AdminApi {
  final ApiClient _client;
  AdminApi(this._client);

  Future<ImportedCatalogItem> importMovie(int tmdbId) =>
      _client.guard(() async {
        final response = await _client.dio.post(
          '/api/v1/admin/import/movie/$tmdbId',
        );
        return ImportedCatalogItem.fromJson(
          response.data as Map<String, dynamic>,
        );
      });

  Future<ImportedCatalogItem> importSeries(int tmdbId) =>
      _client.guard(() async {
        final response = await _client.dio.post(
          '/api/v1/admin/import/series/$tmdbId',
        );
        return ImportedCatalogItem.fromJson(
          response.data as Map<String, dynamic>,
        );
      });

  Future<ImportedCatalogItem> importEpisode({
    required int seriesTmdbId,
    required int seasonNumber,
    required int episodeNumber,
  }) => _client.guard(() async {
    final response = await _client.dio.post(
      '/api/v1/admin/import/episode',
      data: {
        'seriesTmdbId': seriesTmdbId,
        'seasonNumber': seasonNumber,
        'episodeNumber': episodeNumber,
      },
    );
    return ImportedCatalogItem.fromJson(response.data as Map<String, dynamic>);
  });

  Future<ImportRunResult> importMoviesByYear({
    required int year,
    required int pages,
  }) => _client.guard(() async {
    final response = await _client.dio.post(
      '/api/v1/admin/import/movies',
      queryParameters: {'year': year, 'pages': pages},
    );
    return ImportRunResult.fromJson(response.data as Map<String, dynamic>);
  });

  Future<ImportRunResult> importSeriesByYear({
    required int year,
    required int pages,
  }) => _client.guard(() async {
    final response = await _client.dio.post(
      '/api/v1/admin/import/series',
      queryParameters: {'year': year, 'pages': pages},
    );
    return ImportRunResult.fromJson(response.data as Map<String, dynamic>);
  });
}
