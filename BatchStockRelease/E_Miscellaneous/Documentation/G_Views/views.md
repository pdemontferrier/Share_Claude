# Vues XAML

Chaque vue WPF est associée à un fichier `.xaml` et un `.xaml.cs`.

## Exemple :
`Page31.xaml` — Sortie de barres de chutes.

### Structure :
- Titre : `{x:Static resx:P31_00}`
- Champs affichés : `Référence`, `Couleur`, `Quantité`, `Destination`
- Commandes liées : `Valider`, `Suivant`, `Annuler`

### Lien ViewModel :
```xml
<DataContext>
    <viewModels:VM_Page31 />
</DataContext>
