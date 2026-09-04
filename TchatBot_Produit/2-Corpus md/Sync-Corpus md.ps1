<#
.SYNOPSIS
    Reconstruit le dossier plat "All" a partir de l'arborescence produit du
    corpus Markdown, pour rechargement complet du RAG.

.DESCRIPTION
    Projet : Chatbot ADV - TRYBA

    Le corpus est maintenu sous deux formes simultanees :
      - une arborescence par gamme et par type (edition, tracabilite)
      - un dossier plat "All" (import dans le moteur RAG)

    Le dossier cible est VIDE puis reconstruit a chaque execution. Il reflete
    donc toujours exactement l'etat courant de l'arborescence : un fichier
    supprime ou renomme en amont disparait de la cible, et ne peut pas etre
    reinjecte par erreur dans la bibliotheque.

    Le parcours couvre tous les sous-repertoires du corpus a l'EXCEPTION du
    dossier cible.

    Controle prealable : deux fichiers homonymes dans des gammes differentes
    s'ecraseraient silencieusement lors de la mise a plat. Le script s'arrete
    avant toute ecriture si le cas se presente.

.PARAMETER Corpus
    Racine du corpus contenant l'arborescence produit et le dossier cible.

.PARAMETER NomCible
    Nom du sous-dossier plat. Par defaut : All.

.PARAMETER Simulation
    Mode "a blanc" : affiche ce qui serait fait, sans rien ecrire sur disque.

.PARAMETER Journal
    Ecrit un journal CSV horodate a la racine du corpus.

.EXAMPLE
    .\Sync-Corpus_md.ps1 -Simulation
    Controle prealable, aucune ecriture.

.EXAMPLE
    .\Sync-Corpus_md.ps1
    Reconstruction effective du dossier All.
#>

[CmdletBinding()]
param(
    [string] $Corpus = 'D:\3_Dev_Projects\Dev_105\01_Code_Files_Dev\TchatBot_Produit\2-Corpus md',
    [string] $NomCible = 'All',
    [switch] $Simulation,
    [switch] $Journal
)

# Volontairement pas de $ErrorActionPreference global : le script est concu
# pour etre lance en dot-sourcing, et modifierait alors la session appelante.
# Les operations sensibles portent -ErrorAction Stop individuellement.

# Affichage correct des accents dans la console
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch { }

# ---------------------------------------------------------------------------
# 1. Resolution des chemins
# ---------------------------------------------------------------------------

if (-not (Test-Path -LiteralPath $Corpus -PathType Container)) {
    Write-Host ""
    Write-Host "ARRET : corpus introuvable." -ForegroundColor Red
    Write-Host "  $Corpus" -ForegroundColor Red
    return
}

$Corpus = (Resolve-Path -LiteralPath $Corpus).Path.TrimEnd('\')
$Cible  = Join-Path $Corpus $NomCible

Write-Host ""
Write-Host "=== Reconstruction du corpus plat - Chatbot ADV ===" -ForegroundColor Cyan
Write-Host "Corpus        : $Corpus"
Write-Host "Cible         : $Cible"
Write-Host ("Mode          : {0}" -f $(if ($Simulation) { 'SIMULATION' } else { 'RECONSTRUCTION REELLE' }))
Write-Host ""

if (-not (Test-Path -LiteralPath $Cible -PathType Container)) {
    if ($Simulation) {
        Write-Host "Le dossier cible sera cree : $Cible" -ForegroundColor Yellow
    } else {
        New-Item -ItemType Directory -Path $Cible -Force | Out-Null
        Write-Host "Dossier cible cree : $Cible" -ForegroundColor Green
    }
    Write-Host ""
}

# ---------------------------------------------------------------------------
# 2. Inventaire des fichiers Markdown, hors dossier cible
# ---------------------------------------------------------------------------

$sousDossiers = @(
    Get-ChildItem -LiteralPath $Corpus -Directory |
    Where-Object { $_.Name -ne $NomCible }
)

$fichiers = @()
foreach ($d in $sousDossiers) {
    $fichiers += @(Get-ChildItem -LiteralPath $d.FullName -Filter '*.md' -File -Recurse)
}
$fichiers = @($fichiers | Sort-Object FullName)

# Fichiers poses directement a la racine du corpus : hors perimetre, signales
$aLaRacine = @(Get-ChildItem -LiteralPath $Corpus -Filter '*.md' -File)

if ($fichiers.Count -eq 0) {
    Write-Warning "Aucun fichier .md trouve dans l'arborescence produit. Rien a faire."
    return
}

Write-Host ("Sous-repertoires parcourus : {0}" -f $sousDossiers.Count) -ForegroundColor Green
Write-Host ("Fichiers .md detectes      : {0}" -f $fichiers.Count) -ForegroundColor Green

if ($aLaRacine.Count -gt 0) {
    Write-Host ""
    Write-Host ("AVERTISSEMENT : {0} fichier(s) .md a la racine du corpus, hors arborescence produit." -f $aLaRacine.Count) -ForegroundColor Yellow
    Write-Host "Ils ne sont PAS copies. Les classer dans la gamme correspondante." -ForegroundColor Yellow
    foreach ($f in $aLaRacine) { Write-Host ("  - {0}" -f $f.Name) -ForegroundColor Yellow }
}
Write-Host ""

# ---------------------------------------------------------------------------
# 3. Controle des collisions de noms
# ---------------------------------------------------------------------------

$collisions = $fichiers |
    Group-Object -Property Name |
    Where-Object { $_.Count -gt 1 }

if ($collisions) {
    Write-Host "ARRET : collisions de noms detectees." -ForegroundColor Red
    Write-Host "Les fichiers suivants portent le meme nom dans des gammes differentes." -ForegroundColor Red
    Write-Host "Une copie a plat en ecraserait silencieusement une partie." -ForegroundColor Red
    Write-Host ""

    foreach ($c in $collisions) {
        Write-Host ("  [{0}] x{1}" -f $c.Name, $c.Count) -ForegroundColor Yellow
        foreach ($f in $c.Group) {
            $rel = $f.FullName.Substring($Corpus.Length).TrimStart('\')
            Write-Host ("      - {0}" -f $rel)
        }
    }

    Write-Host ""
    Write-Host ("Copie annulee : {0} collision(s) de nom." -f $collisions.Count) -ForegroundColor Red
    Write-Host "Renommer les fichiers en conflit avant de relancer." -ForegroundColor Red
    return
}

# ---------------------------------------------------------------------------
# 4. Purge de la cible
# ---------------------------------------------------------------------------

$aSupprimer = @(Get-ChildItem -LiteralPath $Cible -Filter '*.md' -File)
Write-Host ("Purge cible   : {0} fichier(s) .md supprime(s)" -f $aSupprimer.Count) -ForegroundColor Yellow

if (-not $Simulation) {
    foreach ($f in $aSupprimer) {
        try {
            Remove-Item -LiteralPath $f.FullName -Force -ErrorAction Stop
        }
        catch {
            Write-Host ("  Suppression impossible : {0} -> {1}" -f $f.Name, $_.Exception.Message) -ForegroundColor Red
        }
    }
}
Write-Host ""

# ---------------------------------------------------------------------------
# 5. Copie
# ---------------------------------------------------------------------------

$resultats = New-Object System.Collections.Generic.List[object]
$nbCopies  = 0
$nbErreurs = 0

foreach ($f in $fichiers) {

    $relatif     = $f.FullName.Substring($Corpus.Length).TrimStart('\')
    $destination = Join-Path $Cible $f.Name

    $statut  = 'OK'
    $message = ''

    try {
        if (-not $Simulation) {
            Copy-Item -LiteralPath $f.FullName -Destination $destination -Force -ErrorAction Stop
        }
        $nbCopies++
    }
    catch {
        $statut  = 'ERREUR'
        $message = $_.Exception.Message
        $nbErreurs++
    }

    $couleur = if ($statut -eq 'ERREUR') { 'Red' } else { 'DarkGray' }

    Write-Host ("  [{0}] {1}" -f $statut, $relatif) -ForegroundColor $couleur
    if ($message) { Write-Host ("       -> {0}" -f $message) -ForegroundColor Red }

    $resultats.Add([pscustomobject]@{
        Horodatage   = (Get-Date).ToString('s')
        Statut       = $statut
        SourceRel    = $relatif
        Destination  = $destination
        TailleOctets = $f.Length
        Message      = $message
    })
}

# ---------------------------------------------------------------------------
# 6. Synthese
# ---------------------------------------------------------------------------

Write-Host ""
Write-Host "--- Synthese ---" -ForegroundColor Cyan
Write-Host ("  Supprimes : {0}" -f $aSupprimer.Count)
Write-Host ("  Copies    : {0}" -f $nbCopies) -ForegroundColor Green
if ($nbErreurs -gt 0) {
    Write-Host ("  Erreurs   : {0}" -f $nbErreurs) -ForegroundColor Red
}

if ($Simulation) {
    Write-Host ""
    Write-Host "SIMULATION : aucun fichier n'a ete ecrit." -ForegroundColor Yellow
}
else {
    Write-Host ""
    Write-Host ("  {0} contient {1} fichier(s), prets pour le rechargement du RAG." -f $NomCible, $nbCopies) -ForegroundColor Cyan
}

# ---------------------------------------------------------------------------
# 7. Journal optionnel
# ---------------------------------------------------------------------------

if ($Journal -and -not $Simulation) {
    $nomJournal    = "journal_sync_corpus_{0}.csv" -f (Get-Date -Format 'yyyyMMdd_HHmmss')
    $cheminJournal = Join-Path $Corpus $nomJournal
    $resultats | Export-Csv -LiteralPath $cheminJournal -NoTypeInformation -Encoding UTF8 -Delimiter ';'
    Write-Host ""
    Write-Host ("Journal ecrit : {0}" -f $cheminJournal) -ForegroundColor Cyan
}

Write-Host ""

# Pas de 'exit' : en dot-sourcing il fermerait la session PowerShell appelante.
# Le code de retour est expose via $LASTEXITCODE pour un usage en pipeline.
$global:LASTEXITCODE = if ($nbErreurs -gt 0) { 1 } else { 0 }