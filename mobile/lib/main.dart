import 'package:flutter/material.dart';
import 'app/questigma_app.dart';
import 'app/api_config.dart';
import 'cases/case_api_client.dart';
import 'cases/http_case_repository.dart';

void main() => runApp(
  QuestigmaApp(
    repository: HttpCaseRepository(CaseApiClient(Uri.parse(ApiConfig.baseUrl))),
  ),
);
