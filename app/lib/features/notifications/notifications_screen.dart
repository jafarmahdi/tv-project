import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/localization/app_strings.dart';
import '../../core/models/notification_models.dart';
import '../../core/providers/core_providers.dart';
import '../../core/widgets/async_value_view.dart';

final notificationsListProvider =
    FutureProvider.autoDispose<List<AppNotification>>((ref) => ref.watch(notificationsApiProvider).getAll());

class NotificationsScreen extends ConsumerWidget {
  const NotificationsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final strings = AppStrings.of(context);
    final notifications = ref.watch(notificationsListProvider);

    return Scaffold(
      appBar: AppBar(
        title: Text(strings.notifications),
        actions: [
          TextButton(
            onPressed: () async {
              await ref.read(notificationsApiProvider).markAllRead();
              ref.invalidate(notificationsListProvider);
            },
            child: const Text('Mark all read'),
          ),
        ],
      ),
      body: AsyncValueView(
        value: notifications,
        loadingHeight: 300,
        onRetry: () => ref.invalidate(notificationsListProvider),
        data: (items) => items.isEmpty
            ? const Center(child: Text('No notifications yet.'))
            : ListView.separated(
                itemCount: items.length,
                separatorBuilder: (context, index) => const Divider(height: 1),
                itemBuilder: (context, index) {
                  final n = items[index];
                  return ListTile(
                    leading: CircleAvatar(
                      backgroundColor: n.isRead ? Theme.of(context).colorScheme.surfaceContainerHigh : Theme.of(context).colorScheme.primaryContainer,
                      child: Icon(_iconFor(n.type), size: 18),
                    ),
                    title: Text(n.title, style: TextStyle(fontWeight: n.isRead ? FontWeight.normal : FontWeight.w700)),
                    subtitle: Text(n.body),
                    onTap: () async {
                      if (!n.isRead) {
                        await ref.read(notificationsApiProvider).markRead(n.id);
                        ref.invalidate(notificationsListProvider);
                      }
                    },
                  );
                },
              ),
      ),
    );
  }

  IconData _iconFor(NotificationType type) => switch (type) {
        NotificationType.newEpisode => Icons.new_releases_outlined,
        NotificationType.newSeason => Icons.video_library_outlined,
        NotificationType.upcomingRelease => Icons.event_available_outlined,
        NotificationType.friendActivity => Icons.people_outline,
        NotificationType.achievement => Icons.emoji_events_outlined,
        NotificationType.system => Icons.info_outline,
      };
}
