import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/models/lists_models.dart';
import '../../core/providers/core_providers.dart';

final myListsProvider = FutureProvider.autoDispose<List<UserList>>((ref) => ref.watch(listsApiProvider).getMyLists());

final listItemsProvider =
    FutureProvider.autoDispose.family<List<ListItem>, String>((ref, listId) => ref.watch(listsApiProvider).getItems(listId));
