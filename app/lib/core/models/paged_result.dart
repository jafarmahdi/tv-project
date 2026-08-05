/// Mirrors `WatchLog.Application.Common.Models.PagedResult&lt;T&gt;`.
class PagedResult<T> {
  final List<T> items;
  final int page;
  final int pageSize;
  final int totalCount;
  final int totalPages;

  PagedResult({
    required this.items,
    required this.page,
    required this.pageSize,
    required this.totalCount,
    required this.totalPages,
  });

  factory PagedResult.fromJson(Map<String, dynamic> json, T Function(Map<String, dynamic>) fromJson) =>
      PagedResult<T>(
        items: (json['items'] as List<dynamic>).map((e) => fromJson(e as Map<String, dynamic>)).toList(),
        page: json['page'] as int,
        pageSize: json['pageSize'] as int,
        totalCount: json['totalCount'] as int,
        totalPages: json['totalPages'] as int,
      );
}
