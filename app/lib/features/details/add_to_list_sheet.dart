import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/localization/app_strings.dart';
import '../../core/network/api_exception.dart';
import '../../core/providers/core_providers.dart';

/// A bottom sheet listing the user's lists so they can add a movie/series to one.
Future<void> showAddToListSheet(
  BuildContext context,
  WidgetRef ref, {
  int? movieTmdbId,
  int? seriesTmdbId,
}) {
  return showModalBottomSheet(
    context: context,
    isScrollControlled: true,
    builder: (context) => _AddToListSheet(movieTmdbId: movieTmdbId, seriesTmdbId: seriesTmdbId),
  );
}

class _AddToListSheet extends ConsumerStatefulWidget {
  final int? movieTmdbId;
  final int? seriesTmdbId;
  const _AddToListSheet({this.movieTmdbId, this.seriesTmdbId});

  @override
  ConsumerState<_AddToListSheet> createState() => _AddToListSheetState();
}

class _AddToListSheetState extends ConsumerState<_AddToListSheet> {
  String? _addingListId;

  @override
  Widget build(BuildContext context) {
    final strings = AppStrings.of(context);
    final listsAsync = ref.watch(_myListsFutureProvider);

    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(strings.addToList, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
            const SizedBox(height: 12),
            listsAsync.when(
              data: (lists) => Column(
                mainAxisSize: MainAxisSize.min,
                children: lists
                    .map((list) => ListTile(
                          title: Text(list.name),
                          trailing: _addingListId == list.id
                              ? const SizedBox(width: 18, height: 18, child: CircularProgressIndicator(strokeWidth: 2))
                              : const Icon(Icons.add_circle_outline),
                          onTap: () async {
                            setState(() => _addingListId = list.id);
                            try {
                              await ref.read(listsApiProvider).addItem(
                                    list.id,
                                    movieTmdbId: widget.movieTmdbId,
                                    seriesTmdbId: widget.seriesTmdbId,
                                  );
                              if (context.mounted) {
                                Navigator.of(context).pop();
                                ScaffoldMessenger.of(context)
                                    .showSnackBar(SnackBar(content: Text('Added to ${list.name}')));
                              }
                            } catch (e) {
                              if (context.mounted) {
                                ScaffoldMessenger.of(context).showSnackBar(
                                    SnackBar(content: Text(e is ApiException ? e.message : 'Failed to add.')));
                              }
                            } finally {
                              if (mounted) setState(() => _addingListId = null);
                            }
                          },
                        ))
                    .toList(),
              ),
              loading: () => const Padding(
                padding: EdgeInsets.symmetric(vertical: 24),
                child: Center(child: CircularProgressIndicator()),
              ),
              error: (e, _) => Text(e is ApiException ? e.message : 'Failed to load lists.'),
            ),
          ],
        ),
      ),
    );
  }
}

final _myListsFutureProvider = FutureProvider.autoDispose((ref) => ref.watch(listsApiProvider).getMyLists());
