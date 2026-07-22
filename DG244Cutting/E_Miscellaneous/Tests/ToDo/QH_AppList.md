# Mode Création — Tests unitaires Query Handler QH_AppList
# Référentiel : 0230 + 0231 + 0232-QH. Le 0230 fait foi (§1.4.3, §6.2.1, §6.3.3).

## Cible de test
- Classe sous test : QH_AppList (B_UseCases/Handlers/Queries/QH_AppList.cs).
- Périmètre : la SEULE méthode spécialisée HandleAppAccessibilityAsync.
  Les six lectures héritées du socle (HandleGetByIdAsync,
  HandleGetFirstOrDefaultAsync, HandleGetAnyAsync, HandleGetAllAsync,
  HandleGetAllAsNoTrackingAsync, HandleGetFilteredAsync) sont HORS
  périmètre : elles relèvent des tests unitaires de QH_Generic<T>.

## Régime
- Régime unitaire (§6.2.1, §6.3.3) : xUnit, mocks Moq.
- Mocks :
    Mock<IR_Generic<AppList>> (MockBehavior.Strict) ;
    Mock<IS_ExClassifier> (MockBehavior.Strict).
- Le repository est observé via l'API IR_Generic<AppList>.GetFilteredAsync
  (car HandleGetFilteredAsync du socle y délègue) ; aucun accès EF Core
  ni DbContext n'est instancié.

## Fichiers joints au fil
- QH_AppList.cs (cible).
- IQ_AppList.cs (contrat de la cible).
- IR_Generic.cs (source de contrôle, à joindre uniquement si absente du
  contexte projet permanent).

## Cas de test à couvrir (exhaustifs sur la méthode spécialisée)
1. Précondition structurelle BU_ER_02 — appId <= 0.
     Données : appId ∈ { -1, 0 }.
     Attendu : Ex_Business levée, ErrorCode == BU_ER_02, libellé exact
       « L'identifiant d'application fourni pour AppList est invalide :
       {appId}. Doit être strictement positif. » ; repository JAMAIS
       sollicité (Times.Never sur GetFilteredAsync) ; classifier JAMAIS
       sollicité.

2. Délégation au socle avec prédicat correct — match positif.
     Données : appId = 7 ; repository configuré pour retourner une liste
       contenant au moins un AppList vérifiant le prédicat composite.
     Attendu : retour true ; GetFilteredAsync appelé EXACTEMENT une fois
       avec une CallChain contenant les segments « QH_AppList » et
       « HandleAppAccessibilityAsync » ; predicate effectivement appliqué
       à un AppList témoin { Id = 7, Accessible = true, IsDeleted = false }
       et retournant true sur ce témoin ; predicate retournant false sur
       au moins un témoin négatif (ex. { Id = 7, Accessible = false,
       IsDeleted = false }, { Id = 7, Accessible = true, IsDeleted = true },
       { Id = 8, Accessible = true, IsDeleted = false }).

3. Réduction LINQ-to-Objects — résultat vide.
     Données : repository retourne une liste vide.
     Attendu : retour false ; classifier JAMAIS sollicité.

4. Réduction LINQ-to-Objects — match unique.
     Données : repository retourne une liste à un élément.
     Attendu : retour true.

5. Propagation/redoublement de CallChain.
     Données : Caller = "VM_Test" ; capture de la CallChain transmise à
       GetFilteredAsync via callback Moq.
     Attendu : la chaîne capturée vaut exactement
       "VM_Test > QH_AppList > HandleAppAccessibilityAsync" (la
       contribution du segment hérité s'opère DANS HandleGetFilteredAsync
       du socle et est observée du côté repository sous la forme
       "VM_Test > QH_AppList > HandleAppAccessibilityAsync > QH_AppList >
       HandleGetFilteredAsync"). Vérifier les deux niveaux selon la
       granularité d'observation choisie.

6. Priorité catch (OperationCanceledException) — annulation amont.
     Données : ct déjà annulé avant l'appel (CancellationToken
       canceled) ; appId valide.
     Attendu : OperationCanceledException levée et propagée intacte ;
       classifier JAMAIS sollicité (priorité du catch
       OperationCanceledException sur catch (Exception), R-4.6.13).

7. Requalification terminale via _classifier réinjecté.
     Données : repository configuré pour lever une InvalidOperationException
       arbitraire ; classifier configuré pour retourner une
       Ex_Infrastructure témoin lorsqu'invoqué avec la CallChain attendue
       et l'exception d'origine.
     Attendu : l'exception levée par sut est l'Ex_Infrastructure témoin
       (le _classifier RÉINJECTÉ dans QH_AppList a bien été utilisé, et
       non un quelconque classifier de la classe de base — qui est
       private et inaccessible) ; classifier.Execute appelé EXACTEMENT
       une fois avec la CallChain attendue.

8. Propagation intacte d'Ex_Business non structurelle.
     Données : repository configuré pour lever une Ex_Business arbitraire
       (par exemple BU_ER_01 simulant un cas où GetFilteredAsync du socle
       l'aurait elle-même levée sur un prédicat null — cas théorique pour
       vérifier la propagation).
     Attendu : l'Ex_Business est propagée INTACTE (catch (Ex_Business) {
       throw; }), sans requalification par classifier.

9. Propagation intacte d'Ex_Infrastructure.
     Données : repository configuré pour lever une Ex_Infrastructure.
     Attendu : l'Ex_Infrastructure est propagée INTACTE, sans
       requalification par classifier.

## Livrables attendus
- Livrable 1 : la classe de test QH_AppList_Tests, complète et compilable,
  résidant en <Projet.Tests>.Unit.B_UseCases.Handlers.Queries
  (parité du namespace de la cible).
- Livrable 2 : la checklist de conformité tests (§5 du 0232-QH si
  applicable, à défaut checklist générique des tests unitaires §6 du
  0230).

## Rappels de posture
- Aucun appel EF Core, aucun DbContext, aucune base de données.
- Strict adherence au régime unitaire : seuls IR_Generic<AppList> et
  IS_ExClassifier sont mockés ; aucune autre dépendance.
- Verify systématique des invocations attendues (Times.Once / Times.Never)
  et VerifyAll en fin de test lorsque pertinent.
- Aucune dépendance entre tests ; chaque test instancie son propre sut
  via la fabrique CreateSut().