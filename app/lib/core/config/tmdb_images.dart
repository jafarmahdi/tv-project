/// TMDB image path helpers. Paths returned by the API (e.g. `/abc123.jpg`) are
/// relative — the actual base URL depends on the size variant requested.
class TmdbImages {
  TmdbImages._();

  static const _base = 'https://image.tmdb.org/t/p';

  static String? poster(String? path, {String size = 'w342'}) => path == null ? null : '$_base/$size$path';
  static String? backdrop(String? path, {String size = 'w780'}) => path == null ? null : '$_base/$size$path';
  static String? profile(String? path, {String size = 'w185'}) => path == null ? null : '$_base/$size$path';
  static String? still(String? path, {String size = 'w300'}) => path == null ? null : '$_base/$size$path';
  static String? logo(String? path, {String size = 'w92'}) => path == null ? null : '$_base/$size$path';
}
