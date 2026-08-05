/// A normalized error surfaced from the API — maps the backend's
/// ProblemDetails-shaped error bodies (see `ExceptionHandlingMiddleware`)
/// into something screens can display directly.
class ApiException implements Exception {
  final int? statusCode;
  final String message;
  final Map<String, List<String>>? fieldErrors;

  ApiException({required this.message, this.statusCode, this.fieldErrors});

  factory ApiException.fromResponseData(int? statusCode, dynamic data) {
    if (data is Map<String, dynamic>) {
      final detail = data['detail'] as String?;
      final title = data['title'] as String?;
      final rawErrors = data['errors'];
      Map<String, List<String>>? fieldErrors;
      if (rawErrors is Map) {
        fieldErrors = rawErrors.map(
          (key, value) => MapEntry(key.toString(), (value as List<dynamic>).map((e) => e.toString()).toList()),
        );
      }
      return ApiException(
        statusCode: statusCode,
        message: detail ?? title ?? 'Something went wrong.',
        fieldErrors: fieldErrors,
      );
    }
    return ApiException(statusCode: statusCode, message: 'Something went wrong.');
  }

  @override
  String toString() => message;
}
