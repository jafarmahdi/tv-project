class ImportedCatalogItem {
  final String localId;
  final String entityType;
  final String title;
  final String reference;

  ImportedCatalogItem({
    required this.localId,
    required this.entityType,
    required this.title,
    required this.reference,
  });

  factory ImportedCatalogItem.fromJson(Map<String, dynamic> json) =>
      ImportedCatalogItem(
        localId: json['localId'] as String,
        entityType: json['entityType'] as String,
        title: json['title'] as String,
        reference: json['reference'] as String,
      );
}

class ImportRunResult {
  final int year;
  final int pagesRequested;
  final int itemsDiscovered;
  final int itemsImported;
  final List<String> errors;

  ImportRunResult({
    required this.year,
    required this.pagesRequested,
    required this.itemsDiscovered,
    required this.itemsImported,
    required this.errors,
  });

  factory ImportRunResult.fromJson(Map<String, dynamic> json) =>
      ImportRunResult(
        year: json['year'] as int,
        pagesRequested: json['pagesRequested'] as int,
        itemsDiscovered: json['itemsDiscovered'] as int,
        itemsImported: json['itemsImported'] as int,
        errors: (json['errors'] as List<dynamic>).cast<String>(),
      );
}
