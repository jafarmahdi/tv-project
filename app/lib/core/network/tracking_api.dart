import 'api_client.dart';
import '../models/tracking_models.dart';

class TrackingApi {
  final ApiClient _client;
  TrackingApi(this._client);

  Future<void> markEpisode({
    required int seriesTmdbId,
    required int seasonNumber,
    required int episodeNumber,
    required EpisodeWatchStatus status,
  }) =>
      _client.guard(() => _client.dio.post('/api/v1/tracking/episodes', data: {
            'seriesTmdbId': seriesTmdbId,
            'seasonNumber': seasonNumber,
            'episodeNumber': episodeNumber,
            'status': status.toJson(),
          }));

  Future<void> toggleEpisodeFavorite({
    required int seriesTmdbId,
    required int seasonNumber,
    required int episodeNumber,
    required bool isFavorite,
  }) =>
      _client.guard(() => _client.dio.post('/api/v1/tracking/episodes/favorite', data: {
            'seriesTmdbId': seriesTmdbId,
            'seasonNumber': seasonNumber,
            'episodeNumber': episodeNumber,
            'isFavorite': isFavorite,
          }));

  Future<SeasonProgress> seasonProgress(int seriesTmdbId, int seasonNumber) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/tracking/episodes/series/$seriesTmdbId/seasons/$seasonNumber');
        return SeasonProgress.fromJson(response.data as Map<String, dynamic>);
      });

  Future<NextEpisode?> nextEpisode(int seriesTmdbId) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/tracking/episodes/series/$seriesTmdbId/next');
        if (response.data == null) return null;
        return NextEpisode.fromJson(response.data as Map<String, dynamic>);
      });

  Future<void> markMovie({required int movieTmdbId, required bool isWatched}) => _client.guard(
      () => _client.dio.post('/api/v1/tracking/movies', data: {'movieTmdbId': movieTmdbId, 'isWatched': isWatched}));

  Future<void> toggleMovieFavorite({required int movieTmdbId, required bool isFavorite}) => _client.guard(
      () => _client.dio.post('/api/v1/tracking/movies/favorite', data: {'movieTmdbId': movieTmdbId, 'isFavorite': isFavorite}));
}
