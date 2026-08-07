import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/models/engagement_models.dart';
import '../../core/providers/core_providers.dart';

typedef EngagementKey = ({TargetType targetType, String targetId});

final ratingSummaryProvider = FutureProvider.autoDispose
    .family<RatingSummary, EngagementKey>(
      (ref, key) => ref
          .watch(ratingsApiProvider)
          .getSummary(targetType: key.targetType, targetId: key.targetId),
    );

final commentsProvider = FutureProvider.autoDispose
    .family<List<CommentEntry>, EngagementKey>(
      (ref, key) => ref
          .watch(socialApiProvider)
          .getComments(targetType: key.targetType, targetId: key.targetId),
    );
