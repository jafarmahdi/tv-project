// Smoke test: the app boots without throwing and shows the splash screen
// while the initial session check (which needs a real backend) is pending.

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_test/flutter_test.dart';

import 'package:watchlog/main.dart';

void main() {
  testWidgets('App boots and shows the splash screen', (WidgetTester tester) async {
    await tester.pumpWidget(const ProviderScope(child: WatchLogApp()));
    await tester.pump();

    expect(find.text('WatchLog'), findsOneWidget);
  });
}
