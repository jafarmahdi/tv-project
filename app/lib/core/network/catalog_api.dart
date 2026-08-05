import 'api_client.dart';
import '../models/catalog_models.dart';
import '../models/paged_result.dart';

class CatalogApi {
  final ApiClient _client;
  CatalogApi(this._client);

  Future<PagedResult<MovieSummary>> searchMovies(String query, {int page = 1}) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/movies/search', queryParameters: {'query': query, 'page': page});
        return PagedResult.fromJson(response.data as Map<String, dynamic>, MovieSummary.fromJson);
      });

  Future<PagedResult<SeriesSummary>> searchSeries(String query, {int page = 1}) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/series/search', queryParameters: {'query': query, 'page': page});
        return PagedResult.fromJson(response.data as Map<String, dynamic>, SeriesSummary.fromJson);
      });

  Future<PagedResult<MovieSummary>> trendingMovies({int page = 1}) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/movies/trending', queryParameters: {'page': page});
        return PagedResult.fromJson(response.data as Map<String, dynamic>, MovieSummary.fromJson);
      });

  Future<PagedResult<SeriesSummary>> trendingSeries({int page = 1}) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/series/trending', queryParameters: {'page': page});
        return PagedResult.fromJson(response.data as Map<String, dynamic>, SeriesSummary.fromJson);
      });

  Future<MovieDetail> movieDetail(int tmdbId) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/movies/$tmdbId');
        return MovieDetail.fromJson(response.data as Map<String, dynamic>);
      });

  Future<SeriesDetail> seriesDetail(int tmdbId) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/series/$tmdbId');
        return SeriesDetail.fromJson(response.data as Map<String, dynamic>);
      });

  Future<SeasonDetail> season(int seriesTmdbId, int seasonNumber) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/series/$seriesTmdbId/seasons/$seasonNumber');
        return SeasonDetail.fromJson(response.data as Map<String, dynamic>);
      });

  Future<PagedResult<MovieSummary>> similarMovies(int tmdbId) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/movies/$tmdbId/similar');
        return PagedResult.fromJson(response.data as Map<String, dynamic>, MovieSummary.fromJson);
      });

  Future<PagedResult<SeriesSummary>> similarSeries(int tmdbId) => _client.guard(() async {
        final response = await _client.dio.get('/api/v1/series/$tmdbId/similar');
        return PagedResult.fromJson(response.data as Map<String, dynamic>, SeriesSummary.fromJson);
      });

  Future<List<WatchProvider>> movieWatchProviders(int tmdbId, {String region = 'US'}) => _client.guard(() async {
        final response =
            await _client.dio.get('/api/v1/movies/$tmdbId/watch-providers', queryParameters: {'region': region});
        return (response.data as List<dynamic>).map((e) => WatchProvider.fromJson(e as Map<String, dynamic>)).toList();
      });

  Future<List<WatchProvider>> seriesWatchProviders(int tmdbId, {String region = 'US'}) => _client.guard(() async {
        final response =
            await _client.dio.get('/api/v1/series/$tmdbId/watch-providers', queryParameters: {'region': region});
        return (response.data as List<dynamic>).map((e) => WatchProvider.fromJson(e as Map<String, dynamic>)).toList();
      });
}
