import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/localization/app_strings.dart';
import '../../core/models/engagement_models.dart';
import '../../core/network/api_exception.dart';
import '../../core/providers/core_providers.dart';
import '../../core/theme/app_theme.dart';
import 'engagement_providers.dart';

class EngagementSection extends ConsumerStatefulWidget {
  final TargetType targetType;
  final String targetId;
  final String? title;
  final String? commentHint;
  final bool compact;

  const EngagementSection({
    super.key,
    required this.targetType,
    required this.targetId,
    this.title,
    this.commentHint,
    this.compact = false,
  });

  @override
  ConsumerState<EngagementSection> createState() => _EngagementSectionState();
}

class _EngagementSectionState extends ConsumerState<EngagementSection> {
  final _commentController = TextEditingController();
  bool _submittingComment = false;
  bool _submittingRating = false;

  EngagementKey get _engagementKey =>
      (targetType: widget.targetType, targetId: widget.targetId);

  @override
  void dispose() {
    _commentController.dispose();
    super.dispose();
  }

  Future<void> _submitRating(int score) async {
    setState(() => _submittingRating = true);
    try {
      await ref
          .read(ratingsApiProvider)
          .rate(
            targetType: widget.targetType,
            targetId: widget.targetId,
            score: score,
          );
      ref.invalidate(ratingSummaryProvider(_engagementKey));
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(AppStrings.of(context).ratingSaved)),
        );
      }
    } catch (e) {
      _showError(e);
    } finally {
      if (mounted) setState(() => _submittingRating = false);
    }
  }

  Future<void> _submitComment() async {
    final body = _commentController.text.trim();
    if (body.isEmpty) return;

    setState(() => _submittingComment = true);
    try {
      await ref
          .read(socialApiProvider)
          .addComment(
            targetType: widget.targetType,
            targetId: widget.targetId,
            body: body,
          );
      _commentController.clear();
      ref.invalidate(commentsProvider(_engagementKey));
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(AppStrings.of(context).commentPosted)),
        );
      }
    } catch (e) {
      _showError(e);
    } finally {
      if (mounted) setState(() => _submittingComment = false);
    }
  }

  void _showError(Object error) {
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(
          error is ApiException ? error.message : 'Something went wrong.',
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final strings = AppStrings.of(context);
    final scheme = Theme.of(context).colorScheme;
    final rating = ref.watch(ratingSummaryProvider(_engagementKey));
    final comments = ref.watch(commentsProvider(_engagementKey));
    final title = widget.title ?? strings.community;

    return Card(
      margin: EdgeInsets.zero,
      child: Padding(
        padding: EdgeInsets.all(widget.compact ? 16 : 18),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: scheme.primaryContainer,
                    borderRadius: BorderRadius.circular(AppTheme.radiusSm),
                  ),
                  child: Icon(
                    Icons.forum_outlined,
                    color: scheme.onPrimaryContainer,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Text(
                    title,
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.w800,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 18),
            Text(
              strings.yourRating,
              style: Theme.of(
                context,
              ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w700),
            ),
            const SizedBox(height: 10),
            rating.when(
              data: (summary) => Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Expanded(
                        child: _ScorePicker(
                          score: summary.myScore,
                          compact: widget.compact,
                          onSelected: _submittingRating ? null : _submitRating,
                        ),
                      ),
                      if (_submittingRating)
                        const Padding(
                          padding: EdgeInsetsDirectional.only(start: 8, top: 6),
                          child: SizedBox(
                            width: 18,
                            height: 18,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          ),
                        ),
                    ],
                  ),
                  const SizedBox(height: 10),
                  _RatingSummaryCard(summary: summary),
                ],
              ),
              loading: () => const Padding(
                padding: EdgeInsets.symmetric(vertical: 12),
                child: SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                ),
              ),
              error: (_, _) =>
                  Text(strings.retry, style: TextStyle(color: scheme.error)),
            ),
            const SizedBox(height: 20),
            Text(
              strings.writeComment,
              style: Theme.of(
                context,
              ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w700),
            ),
            const SizedBox(height: 10),
            TextField(
              controller: _commentController,
              minLines: 2,
              maxLines: 4,
              textInputAction: TextInputAction.newline,
              decoration: InputDecoration(
                hintText: widget.commentHint ?? strings.shareThoughts,
              ),
            ),
            const SizedBox(height: 12),
            Align(
              alignment: AlignmentDirectional.centerEnd,
              child: FilledButton(
                onPressed: _submittingComment ? null : _submitComment,
                child: _submittingComment
                    ? const SizedBox(
                        width: 18,
                        height: 18,
                        child: CircularProgressIndicator(strokeWidth: 2),
                      )
                    : Text(strings.post),
              ),
            ),
            const SizedBox(height: 18),
            comments.when(
              data: (entries) => entries.isEmpty
                  ? _EmptyDiscussion(
                      title: strings.noCommentsYet,
                      subtitle: strings.startConversation,
                    )
                  : Column(
                      children: entries
                          .map((entry) => _CommentCard(entry: entry))
                          .toList(),
                    ),
              loading: () => const Padding(
                padding: EdgeInsets.symmetric(vertical: 12),
                child: SizedBox(
                  width: 20,
                  height: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                ),
              ),
              error: (_, _) =>
                  Text(strings.retry, style: TextStyle(color: scheme.error)),
            ),
          ],
        ),
      ),
    );
  }
}

class _ScorePicker extends StatelessWidget {
  final int? score;
  final bool compact;
  final ValueChanged<int>? onSelected;

  const _ScorePicker({
    required this.score,
    required this.compact,
    required this.onSelected,
  });

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;

    return Wrap(
      spacing: compact ? 0 : 2,
      runSpacing: 2,
      children: List.generate(10, (index) {
        final value = index + 1;
        final isSelected = score != null && value <= score!;
        return IconButton(
          onPressed: onSelected == null ? null : () => onSelected!(value),
          tooltip: '$value/10',
          visualDensity: VisualDensity.compact,
          padding: EdgeInsets.zero,
          constraints: BoxConstraints.tightFor(
            width: compact ? 28 : 32,
            height: compact ? 28 : 32,
          ),
          icon: Icon(
            isSelected ? Icons.star_rounded : Icons.star_border_rounded,
            size: compact ? 18 : 20,
            color: isSelected ? AppTheme.accent : scheme.onSurfaceVariant,
          ),
        );
      }),
    );
  }
}

class _RatingSummaryCard extends StatelessWidget {
  final RatingSummary summary;

  const _RatingSummaryCard({required this.summary});

  @override
  Widget build(BuildContext context) {
    final strings = AppStrings.of(context);
    final scheme = Theme.of(context).colorScheme;

    return Container(
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: scheme.surfaceContainerHighest,
        borderRadius: BorderRadius.circular(AppTheme.radiusMd),
      ),
      child: Row(
        children: [
          Container(
            width: 44,
            height: 44,
            decoration: BoxDecoration(
              color: scheme.tertiaryContainer,
              borderRadius: BorderRadius.circular(AppTheme.radiusSm),
            ),
            child: Icon(Icons.star_rounded, color: scheme.onTertiaryContainer),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  summary.count == 0
                      ? strings.beFirstToRate
                      : '${summary.average.toStringAsFixed(1)} / 10',
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w800,
                  ),
                ),
                const SizedBox(height: 2),
                Text(
                  summary.count == 0
                      ? strings.communityRating
                      : '${summary.count} ${strings.ratingsCount}',
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: scheme.onSurfaceVariant,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _EmptyDiscussion extends StatelessWidget {
  final String title;
  final String subtitle;

  const _EmptyDiscussion({required this.title, required this.subtitle});

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: scheme.surfaceContainerHigh,
        borderRadius: BorderRadius.circular(AppTheme.radiusMd),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: Theme.of(
              context,
            ).textTheme.bodyLarge?.copyWith(fontWeight: FontWeight.w700),
          ),
          const SizedBox(height: 4),
          Text(
            subtitle,
            style: Theme.of(
              context,
            ).textTheme.bodySmall?.copyWith(color: scheme.onSurfaceVariant),
          ),
        ],
      ),
    );
  }
}

class _CommentCard extends StatelessWidget {
  final CommentEntry entry;

  const _CommentCard({required this.entry});

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;

    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: scheme.surfaceContainerHigh,
        borderRadius: BorderRadius.circular(AppTheme.radiusMd),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              CircleAvatar(
                radius: 18,
                backgroundColor: scheme.primaryContainer,
                backgroundImage: entry.userAvatarUrl != null
                    ? NetworkImage(entry.userAvatarUrl!)
                    : null,
                child: entry.userAvatarUrl == null
                    ? Text(
                        entry.userDisplayName.isEmpty
                            ? '?'
                            : entry.userDisplayName[0].toUpperCase(),
                        style: TextStyle(
                          color: scheme.onPrimaryContainer,
                          fontWeight: FontWeight.w700,
                        ),
                      )
                    : null,
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      entry.userDisplayName,
                      style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                    Text(
                      _formatDate(entry.createdAt),
                      style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: scheme.onSurfaceVariant,
                      ),
                    ),
                  ],
                ),
              ),
              if (entry.likeCount > 0)
                Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(
                      Icons.favorite_rounded,
                      size: 14,
                      color: scheme.primary,
                    ),
                    const SizedBox(width: 4),
                    Text(
                      '${entry.likeCount}',
                      style: Theme.of(context).textTheme.bodySmall,
                    ),
                  ],
                ),
            ],
          ),
          const SizedBox(height: 12),
          Text(entry.body, style: Theme.of(context).textTheme.bodyMedium),
        ],
      ),
    );
  }

  String _formatDate(DateTime value) {
    final month = value.month.toString().padLeft(2, '0');
    final day = value.day.toString().padLeft(2, '0');
    return '${value.year}-$month-$day';
  }
}
