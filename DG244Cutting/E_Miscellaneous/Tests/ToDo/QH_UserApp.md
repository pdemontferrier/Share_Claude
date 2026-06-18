# Mode Test unitaire — Famille Query Handlers (QH_)
# Référentiel : 023 §6.2.1 / §6.3.3 + 0232-QH §3.2.3. Le 023 fait foi.
# Fil de production de référence : QH_UserApp_Extension (à référencer dans le Commit 2).

## Class cible
- Implémentation testée : QH_UserApp (B_UseCases/Handlers/Queries/)
- Contrat associé    : IQ_UserApp  (A_Domain/Interfaces/Handlers/Queries/)
- Régime de test     : UNITAIRE strict (xUnit + Moq)
- Mocks              : IR_Generic<UserApp> et IS_ExClassifier en
                       MockBehavior.Strict ; aucune base réelle ou en mémoire.
- Périmètre          : méthodes spécialisées seules. Les six lectures du
                       socle relèvent des tests de QH_Generic<T> (hors fil).

## Périmètre fonctionnel à couvrir

### A. Non-régression — HandleGetByWindowsLoginAsync (lecture spécialisée
   existante, sous-cas (i) — délégation à HandleGetFilteredAsync hérité)

Reprendre intégralement et sans modification les six cas du jeu canonique
porté en §3.2.3 du 0232-QH (couple QH_UserApp / IQ_UserApp) :
  1) Précondition structurelle BU_ER_01 sur windowsLogin null / "" / "   "
     ; repository jamais sollicité (Times.Never).
  2) Match existant — délégation au socle avec CallChain correctement
     enrichie, réduction au premier enregistrement.
  3) Aucun match — retour null.
  4) Propagation de la CallChain — segments Caller, QH_UserApp et
     HandleGetByWindowsLoginAsync présents dans la chaîne reçue par le
     repository (redoublement assumé du segment _callee).
  5) Token déjà annulé — OperationCanceledException sans requalification
     (priorité du catch (OperationCanceledException), R-4.6.13).
  6) Exception non typée levée par le repository — requalification via
     IS_ExClassifier (Ex_Infrastructure renvoyée par le classifier).

### B. Lecture ajoutée — HandleGetFullNameByIdAsync (sous-cas (i) —
   délégation à HandleGetByIdAsync hérité)

Couvrir explicitement les neuf cas suivants :

  B1. Précondition structurelle BU_ER_02 — userId <= 0
      [Theory] InlineData(0), InlineData(-1), InlineData(int.MinValue).
      Vérifier : Ex_Business levée, ErrorCode == BU_ER_02, message en
      français contenant la valeur fournie ; le repository n'est PAS
      sollicité (Times.Never sur GetByIdAsync).

  B2. Cas user == null (entité inexistante) → BU_ER_03
      Mock IR_Generic<UserApp>.GetByIdAsync renvoyant Task.FromResult<UserApp?>(null).
      Vérifier : Ex_Business levée, ErrorCode == BU_ER_03, message en
      français contenant la valeur de userId et la mention « Id inexistant
      ou supprimé ».

  B3. Cas user.IsDeleted == true (entité logiquement supprimée) → BU_ER_03
      Mock renvoyant un UserApp { Id = 42, FirstName = "Jean", LastName =
      "Dupont", IsDeleted = true }. Vérifier : Ex_Business levée,
      ErrorCode == BU_ER_03, message identique à B2.

  B4. Garde-fou défensif sur FirstName blanc → valeur de repli
      [Theory] InlineData(null), InlineData(""), InlineData("   ") pour
      FirstName ; LastName = "Dupont", IsDeleted = false. Vérifier :
      retour == $"Utilisateur non identifié : {userId}" ; aucune exception ;
      classifier jamais sollicité.

  B5. Garde-fou défensif sur LastName blanc → valeur de repli
      Symétrique de B4 : FirstName = "Jean", LastName ∈ { null, "", "   " },
      IsDeleted = false. Vérifier : retour == $"Utilisateur non identifié : {userId}".

  B6. Cas nominal — concaténation attendue
      Mock renvoyant UserApp { Id = 42, FirstName = "Jean", LastName =
      "Dupont", IsDeleted = false }. Vérifier : retour == "Jean Dupont"
      (exactement, espace simple).

  B7. Propagation de la CallChain au format normatif
      Capturer la CallChain transmise à _repository.GetByIdAsync via
      .Callback<string, int, CancellationToken>((cc, _, _) => captured = cc).
      Le repository renvoie un UserApp nominal pour court-circuiter le
      reste. Vérifier que la chaîne capturée contient, dans cet ordre :
        - le segment Caller amont,
        - le segment "QH_UserApp" (redoublé : une fois pour le segment
          _callee de HandleGetFullNameByIdAsync, une fois pour celui de
          HandleGetByIdAsync hérité — redoublement normatif §4.15.4),
        - nameof(QH_UserApp.HandleGetFullNameByIdAsync),
        - nameof(QH_Generic<UserApp>.HandleGetByIdAsync) (i.e. "HandleGetByIdAsync").
      Assertion souple : Assert.Contains pour chaque segment, et
      occurrences("QH_UserApp", captured) >= 2.

  B8. Priorité du catch (OperationCanceledException)
      using var cts = new CancellationTokenSource(); cts.Cancel();
      Le repository ne doit pas être sollicité (ct.ThrowIfCancellationRequested
      lève avant). Vérifier : Assert.ThrowsAsync<OperationCanceledException>
      ; classifier jamais sollicité (Times.Never sur Execute).

  B9. Requalification terminale via IS_ExClassifier réinjecté
      Mock IR_Generic<UserApp>.GetByIdAsync levant une
      InvalidOperationException("panne technique simulée").
      Mock IS_ExClassifier.Execute(It.IsAny<string>(), raw) renvoyant une
      Ex_Infrastructure préfabriquée. Vérifier : Assert.ThrowsAsync<Ex_Infrastructure>
      renvoie bien l'instance préfabriquée (Assert.Same) ;
      _classifier.Verify(c => c.Execute(It.IsAny<string>(), raw), Times.Once).

## Conventions de test attendues
- xUnit (Fact / Theory / InlineData), Moq (MockBehavior.Strict).
- Nom de méthode de test au format
  HandleGetFullNameByIdAsync_<Contexte>_<Résultat> (cf. §3.2.3 du 0232-QH).
- Aucun accès à un DbContext, à une base SQLite en mémoire ni à EF Core
  réel : régime unitaire strict (§6.3.3).
- Aucune assertion sur les six lectures héritées du socle : périmètre
  cantonné aux méthodes spécialisées seules.
- Une const string Caller = "VM_Test" partagée par les tests, comme dans
  l'exemple canonique de §3.2.3.

## Livrables attendus du fil de test
- Fichier QH_UserApp_Tests.cs en Tests.Unit.B_UseCases.Handlers.Queries,
  étendu (ou créé) avec les classes/régions/methods couvrant A et B
  ci-dessus, compilable et exécutable.
- Checklist d'attestation d'exécution de la suite (§5.2.5 du 0232-QH —
  Commit 2) : couverture des points BU_ER_02 (B1), BU_ER_03 (B2/B3),
  garde-fou défensif (B4/B5), nominal (B6), CallChain redoublée (B7),
  priorité OperationCanceledException (B8), requalification terminale
  (B9), plus non-régression HandleGetByWindowsLoginAsync (A1-A6).
- Message de Commit 2 référençant explicitement le Commit 1 du fil
  QH_UserApp_Extension, conforme à §5.2.5 du 0232-QH (type = test).

## Rappels de posture
- Aucune dépendance à une base réelle ou à EF Core : tout passe par les
  mocks Moq sur IR_Generic<UserApp> et IS_ExClassifier (§6.3.3 du 023).
- Le redoublement du segment _callee dans la CallChain reçue par le
  repository est normatif (§4.15.4) : un test qui le signalerait comme
  défaut serait lui-même non conforme.
- Aucun test ne couvre les six lectures du socle (relèvent des tests
  dédiés à QH_Generic<T>, hors présent fil).
- Toute divergence détectée entre le code testé et les checklists du
  0232-QH §4 est signalée en clôture du fil, sans correction dans le fil.