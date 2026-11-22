import 'dart:async';
import 'dart:io';

import 'package:flutter_test/flutter_test.dart';
import 'package:questigma/features/investigation/data/case_api_client.dart';

void main() {
  late HttpServer server;

  setUp(() async {
    server = await HttpServer.bind(InternetAddress.loopbackIPv4, 0);
  });

  tearDown(() async {
    await server.close(force: true);
  });

  test('parses the public summary response', () async {
    server.listen((request) {
      expect(request.uri.path, '/api/v1/cases');
      request.response
        ..headers.contentType = ContentType.json
        ..write(
          '[{"id":"4edab805-087b-41fb-bfb6-f5736c6ab0d7","title":"Piloto","synopsis":"Mistério"}]',
        );
      request.response.close();
    });
    final cases = await CaseApiClient(
      Uri.parse('http://127.0.0.1:${server.port}'),
    ).listCases();
    expect(cases.single.id, '4edab805-087b-41fb-bfb6-f5736c6ab0d7');
    expect(cases.single.title, 'Piloto');
    expect(cases.single.synopsis, 'Mistério');
  });

  test('reports a non-success HTTP response', () async {
    server.listen((request) {
      request.response.statusCode = HttpStatus.serviceUnavailable;
      request.response.close();
    });
    expect(
      CaseApiClient(Uri.parse('http://127.0.0.1:${server.port}')).listCases(),
      throwsA(isA<HttpException>()),
    );
  });

  test('times out an unfinished response promptly', () async {
    server.listen((request) {
      expect(request.uri.path, '/api/v1/cases');
      // Keep the response open to exercise the whole-request deadline.
    });
    final client = CaseApiClient(
      Uri.parse('http://127.0.0.1:${server.port}'),
      timeout: const Duration(milliseconds: 50),
    );

    await expectLater(client.listCases(), throwsA(isA<TimeoutException>()));
  });
}
