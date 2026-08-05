import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/models/stats_models.dart';
import '../../core/providers/core_providers.dart';

final myStatsProvider = FutureProvider.autoDispose<UserStats>((ref) => ref.watch(statsApiProvider).getMyStats());
