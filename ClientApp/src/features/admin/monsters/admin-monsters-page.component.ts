import { CommonModule } from '@angular/common';
import { CdkDragDrop, DragDropModule } from '@angular/cdk/drag-drop';
import { Component, OnInit, inject } from '@angular/core';
import { FormArray, FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { MonsterAbilityCatalogItem } from './models/monster-ability-catalog-item';
import { MonsterDetail, MonsterStatValue } from './models/monster-detail';
import { MonsterItemCatalogItem } from './models/monster-item-catalog-item';
import { MonsterSaveRequest } from './models/monster-save-request';
import { MonsterStatCatalogItem } from './models/monster-stat-catalog-item';
import { MonsterSummary } from './models/monster-summary';
import { AdminMonstersService } from './services/admin-monsters.service';

type MonsterEditorStage = 'identity' | 'stats' | 'abilities' | 'drops';

@Component({
  selector: 'app-admin-monsters-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DragDropModule],
  templateUrl: './admin-monsters-page.component.html',
  styleUrl: './admin-monsters-page.component.scss',
})
export class AdminMonstersPageComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly adminMonstersService = inject(AdminMonstersService);

  readonly monsterSearchControl = new FormControl('', { nonNullable: true });
  readonly itemSearchControl = new FormControl('', { nonNullable: true });
  readonly abilitySearchControl = new FormControl('', { nonNullable: true });
  readonly statSearchControl = new FormControl('', { nonNullable: true });

  readonly monsterForm = this.fb.group({
    name: this.fb.control('', { nonNullable: true, validators: [Validators.required] }),
    overallDropChance: this.fb.control(0, {
      nonNullable: true,
      validators: [Validators.required, Validators.min(0), Validators.max(1)],
    }),
    stats: this.fb.array([]),
    abilities: this.fb.array([]),
    drops: this.fb.array([]),
  });

  monsters: MonsterSummary[] = [];
  itemCatalog: MonsterItemCatalogItem[] = [];
  abilityCatalog: MonsterAbilityCatalogItem[] = [];
  statCatalog: MonsterStatCatalogItem[] = [];

  selectedMonsterName: string | null = null;
  readonly collapsedStages = new Set<MonsterEditorStage>();
  isLoading = true;
  isSaving = false;
  isDeleting = false;
  formSubmitted = false;
  loadError: string | null = null;
  submitError: string | null = null;
  successMessage: string | null = null;

  ngOnInit(): void {
    this.loadMonsterData();
  }

  get statsArray(): FormArray {
    return this.monsterForm.get('stats') as FormArray;
  }

  get abilitiesArray(): FormArray {
    return this.monsterForm.get('abilities') as FormArray;
  }

  get dropsArray(): FormArray {
    return this.monsterForm.get('drops') as FormArray;
  }

  get filteredMonsters(): MonsterSummary[] {
    const searchTerm = this.monsterSearchControl.value.trim().toLowerCase();
    if (!searchTerm) {
      return this.monsters;
    }

    return this.monsters.filter((monster) => monster.name.toLowerCase().includes(searchTerm));
  }

  get filteredItemCatalog(): MonsterItemCatalogItem[] {
    const searchTerm = this.itemSearchControl.value.trim().toLowerCase();

    return this.itemCatalog.filter((item) => {
      const matchesSearch =
        !searchTerm ||
        item.name.toLowerCase().includes(searchTerm) ||
        item.category.toLowerCase().includes(searchTerm) ||
        item.itemId.toLowerCase().includes(searchTerm);

      return matchesSearch;
    });
  }

  get filteredAbilityCatalog(): MonsterAbilityCatalogItem[] {
    const searchTerm = this.abilitySearchControl.value.trim().toLowerCase();

    return this.abilityCatalog.filter((ability) => {
      const matchesSearch =
        !searchTerm ||
        ability.name.toLowerCase().includes(searchTerm) ||
        ability.typeName.toLowerCase().includes(searchTerm) ||
        ability.id.toLowerCase().includes(searchTerm);

      return matchesSearch;
    });
  }

  get filteredStatCatalog(): MonsterStatCatalogItem[] {
    const searchTerm = this.statSearchControl.value.trim().toLowerCase();

    return this.statCatalog.filter((stat) => {
      const matchesSearch =
        !searchTerm ||
        stat.name.toLowerCase().includes(searchTerm) ||
        stat.key.toLowerCase().includes(searchTerm);

      return matchesSearch;
    });
  }

  get selectedStatKeys(): Set<string> {
    return new Set(
      this.statsArray.controls
        .map((control) => control.get('key')?.value as string)
        .filter((value) => !!value)
    );
  }

  get selectedAbilityIds(): Set<string> {
    return new Set(
      this.abilitiesArray.controls
        .map((control) => control.get('id')?.value as string)
        .filter((value) => !!value)
    );
  }

  get selectedDropKeys(): Set<string> {
    return new Set(
      this.dropsArray.controls.map((control) =>
        this.composeDropKey(
          control.get('itemTypeName')?.value as string,
          control.get('itemId')?.value as string
        )
      )
    );
  }

  get canDeleteSelected(): boolean {
    return this.selectedMonsterName !== null && !this.isDeleting;
  }

  get isFormInvalid(): boolean {
    return (
      !this.monsterForm.get('name')?.value?.trim() ||
      this.hasInvalidDropChance() ||
      this.hasDuplicateStats() ||
      this.hasDuplicateAbilities() ||
      this.hasDuplicateDrops() ||
      this.hasInvalidDrops()
    );
  }

  get selectedAbilities(): MonsterAbilityCatalogItem[] {
    return this.abilitiesArray.controls
      .map((control) => this.getAbilityById(control.get('id')?.value as string))
      .filter((item): item is MonsterAbilityCatalogItem => item !== null);
  }

  loadMonsterData(selectMonsterName: string | null = this.selectedMonsterName): void {
    this.isLoading = true;
    this.loadError = null;

    forkJoin({
      monsters: this.adminMonstersService.getMonsters(),
      itemCatalog: this.adminMonstersService.getItemCatalog(),
      abilityCatalog: this.adminMonstersService.getAbilityCatalog(),
      statCatalog: this.adminMonstersService.getStatCatalog(),
    }).subscribe({
      next: ({ monsters, itemCatalog, abilityCatalog, statCatalog }) => {
        this.monsters = monsters;
        this.itemCatalog = itemCatalog;
        this.abilityCatalog = abilityCatalog;
        this.statCatalog = statCatalog;

        if (selectMonsterName) {
          this.openMonster(selectMonsterName);
          return;
        }

        this.startCreate();
        this.isLoading = false;
      },
      error: (error: Error) => {
        this.loadError = error.message;
        this.isLoading = false;
      },
    });
  }

  startCreate(): void {
    this.selectedMonsterName = null;
    this.successMessage = null;
    this.submitError = null;
    this.formSubmitted = false;

    this.monsterForm.reset({
      name: '',
      overallDropChance: 0,
    });

    this.clearFormArray(this.statsArray);
    this.clearFormArray(this.abilitiesArray);
    this.clearFormArray(this.dropsArray);
    this.seedDefaultStats();
    this.isLoading = false;
  }

  openMonster(monsterName: string): void {
    this.selectedMonsterName = monsterName;
    this.submitError = null;
    this.successMessage = null;

    this.adminMonstersService.getMonster(monsterName).subscribe({
      next: (monster) => {
        this.patchMonsterForm(monster);
        this.isLoading = false;
      },
      error: (error: Error) => {
        this.loadError = error.message;
        this.isLoading = false;
      },
    });
  }

  onStatDropped(event: CdkDragDrop<unknown>): void {
    this.attachStat(event.item.data as MonsterStatCatalogItem);
  }

  onAbilityDropped(event: CdkDragDrop<unknown>): void {
    this.attachAbility(event.item.data as MonsterAbilityCatalogItem);
  }

  onItemDropped(event: CdkDragDrop<unknown>): void {
    this.attachDropItem(event.item.data as MonsterItemCatalogItem);
  }

  attachStat(stat: MonsterStatCatalogItem): void {
    if (!this.canAttachStat(stat)) {
      return;
    }

    this.statsArray.push(
      this.fb.group({
        key: this.fb.control(stat.key, { nonNullable: true, validators: [Validators.required] }),
        value: this.fb.control(stat.defaultValue, { nonNullable: true, validators: [Validators.required] }),
      })
    );

    this.submitError = null;
  }

  removeStat(index: number): void {
    this.statsArray.removeAt(index);
  }

  attachAbility(ability: MonsterAbilityCatalogItem): void {
    if (!this.canAttachAbility(ability)) {
      return;
    }

    this.abilitiesArray.push(
      this.fb.group({
        id: this.fb.control(ability.id, { nonNullable: true, validators: [Validators.required] }),
      })
    );

    this.submitError = null;
  }

  removeAbility(index: number): void {
    this.abilitiesArray.removeAt(index);
  }

  attachDropItem(item: MonsterItemCatalogItem): void {
    if (!this.canAttachItem(item)) {
      return;
    }

    this.dropsArray.push(
      this.fb.group({
        itemTypeName: this.fb.control(item.typeName, {
          nonNullable: true,
          validators: [Validators.required],
        }),
        itemId: this.fb.control(item.itemId, { nonNullable: true, validators: [Validators.required] }),
        quantity: this.fb.control(1, {
          nonNullable: true,
          validators: [Validators.required, Validators.min(1)],
        }),
        weight: this.fb.control(1, {
          nonNullable: true,
          validators: [Validators.required, Validators.min(0)],
        }),
      })
    );

    if (Number(this.monsterForm.get('overallDropChance')?.value) === 0) {
      this.monsterForm.patchValue({ overallDropChance: 1 });
    }

    this.submitError = null;
  }

  removeDrop(index: number): void {
    this.dropsArray.removeAt(index);
  }

  toggleStage(stage: MonsterEditorStage): void {
    if (this.collapsedStages.has(stage)) {
      this.collapsedStages.delete(stage);
      return;
    }

    this.collapsedStages.add(stage);
  }

  isStageCollapsed(stage: MonsterEditorStage): boolean {
    return this.collapsedStages.has(stage);
  }

  saveMonster(): void {
    this.formSubmitted = true;
    this.submitError = null;
    this.successMessage = null;

    if (this.isFormInvalid) {
      this.submitError = this.resolveValidationMessage();
      return;
    }

    const request = this.buildSaveRequest();
    if (!request) {
      this.submitError = 'Monster data is incomplete.';
      return;
    }

    this.isSaving = true;
    const isCreating = this.selectedMonsterName === null;

    if (isCreating) {
      this.adminMonstersService.createMonster(request).subscribe({
        next: (created) => {
          this.isSaving = false;
          this.successMessage = 'Monster created.';
          this.loadMonsterData(created.name);
        },
        error: (error: Error) => {
          this.isSaving = false;
          this.submitError = error.message;
        },
      });

      return;
    }

    const currentMonsterName = this.selectedMonsterName;
    if (!currentMonsterName) {
      this.isSaving = false;
      this.submitError = 'Current monster name is missing.';
      return;
    }

    this.adminMonstersService.updateMonster(currentMonsterName, request).subscribe({
      next: () => {
        this.isSaving = false;
        this.successMessage = 'Monster updated.';
        this.loadMonsterData(request.name);
      },
      error: (error: Error) => {
        this.isSaving = false;
        this.submitError = error.message;
      },
    });
  }

  deleteMonster(): void {
    if (!this.selectedMonsterName || !window.confirm('Delete this monster?')) {
      return;
    }

    this.isDeleting = true;
    this.submitError = null;
    this.successMessage = null;

    this.adminMonstersService.deleteMonster(this.selectedMonsterName).subscribe({
      next: () => {
        this.isDeleting = false;
        this.successMessage = 'Monster deleted.';
        this.loadMonsterData(null);
      },
      error: (error: Error) => {
        this.isDeleting = false;
        this.submitError = error.message;
      },
    });
  }

  canAttachStat(stat: MonsterStatCatalogItem): boolean {
    return !this.selectedStatKeys.has(stat.key);
  }

  canAttachAbility(ability: MonsterAbilityCatalogItem): boolean {
    return !this.selectedAbilityIds.has(ability.id);
  }

  canAttachItem(item: MonsterItemCatalogItem): boolean {
    return !this.selectedDropKeys.has(this.composeDropKey(item.typeName, item.itemId));
  }

  getStatDefinition(statKey: string): MonsterStatCatalogItem | null {
    return this.statCatalog.find((stat) => stat.key === statKey) ?? null;
  }

  getStatInputStep(statKey: string): string {
    return this.getStatDefinition(statKey)?.valueType === 'integer' ? '1' : '0.01';
  }

  getAbilityById(abilityId: string): MonsterAbilityCatalogItem | null {
    return this.abilityCatalog.find((ability) => ability.id === abilityId) ?? null;
  }

  getDropItem(itemTypeName: string, itemId: string): MonsterItemCatalogItem | null {
    return (
      this.itemCatalog.find((item) => item.typeName === itemTypeName && item.itemId === itemId) ?? null
    );
  }

  getDropChanceOnRoll(index: number): number {
    const totalWeight = this.getTotalDropWeight();
    if (totalWeight <= 0) {
      return 0;
    }

    const weight = Number(this.dropsArray.at(index)?.get('weight')?.value);
    if (Number.isNaN(weight) || weight <= 0) {
      return 0;
    }

    return weight / totalWeight;
  }

  getEffectiveDropChance(index: number): number {
    const overallDropChance = Number(this.monsterForm.get('overallDropChance')?.value);
    if (Number.isNaN(overallDropChance) || overallDropChance <= 0) {
      return 0;
    }

    return overallDropChance * this.getDropChanceOnRoll(index);
  }

  formatPercent(value: number): string {
    return `${(value * 100).toFixed(value >= 0.1 ? 1 : 2)}%`;
  }

  getItemImagePath(imageUrl: string): string {
    if (!imageUrl) {
      return '/item-background.png';
    }

    return /\.(png|jpg|jpeg|webp|gif|svg)$/i.test(imageUrl) ? imageUrl : `${imageUrl}.png`;
  }

  getAbilityImagePath(imageUrl: string): string {
    if (!imageUrl) {
      return '/tooltip-scroll.png';
    }

    return `/test/${imageUrl}`;
  }

  trackMonster(_index: number, monster: MonsterSummary): string {
    return monster.name;
  }

  trackItem(_index: number, item: MonsterItemCatalogItem): string {
    return `${item.typeName}::${item.itemId}`;
  }

  trackAbility(_index: number, ability: MonsterAbilityCatalogItem): string {
    return ability.id;
  }

  trackStat(_index: number, stat: MonsterStatCatalogItem): string {
    return stat.key;
  }

  private patchMonsterForm(monster: MonsterDetail): void {
    this.formSubmitted = false;
    this.monsterForm.patchValue({
      name: monster.name,
      overallDropChance: monster.overallDropChance,
    });

    this.clearFormArray(this.statsArray);
    this.clearFormArray(this.abilitiesArray);
    this.clearFormArray(this.dropsArray);

    monster.stats.forEach((stat) => this.pushStatGroup(stat));
    monster.abilityIds.forEach((abilityId) => {
      this.abilitiesArray.push(
        this.fb.group({
          id: this.fb.control(abilityId, { nonNullable: true, validators: [Validators.required] }),
        })
      );
    });

    monster.drops.forEach((drop) => {
      this.dropsArray.push(
        this.fb.group({
          itemTypeName: this.fb.control(drop.itemTypeName, {
            nonNullable: true,
            validators: [Validators.required],
          }),
          itemId: this.fb.control(drop.itemId, {
            nonNullable: true,
            validators: [Validators.required],
          }),
          quantity: this.fb.control(drop.quantity, {
            nonNullable: true,
            validators: [Validators.required, Validators.min(1)],
          }),
          weight: this.fb.control(drop.weight, {
            nonNullable: true,
            validators: [Validators.required, Validators.min(0)],
          }),
        })
      );
    });
  }

  private pushStatGroup(stat: MonsterStatValue): void {
    this.statsArray.push(
      this.fb.group({
        key: this.fb.control(stat.key, { nonNullable: true, validators: [Validators.required] }),
        value: this.fb.control(stat.value, { nonNullable: true, validators: [Validators.required] }),
      })
    );
  }

  private seedDefaultStats(): void {
    this.statCatalog.forEach((stat) => {
      this.pushStatGroup({
        key: stat.key,
        name: stat.name,
        valueType: stat.valueType,
        value: stat.defaultValue,
      });
    });
  }

  private resolveValidationMessage(): string {
    if (!this.monsterForm.get('name')?.value?.trim()) {
      return 'Monster name is required.';
    }

    if (this.hasInvalidDropChance()) {
      return 'Overall drop chance must stay between 0 and 1.';
    }

    if (this.hasDuplicateStats()) {
      return 'Each stat can only be selected once.';
    }

    if (this.hasDuplicateAbilities()) {
      return 'Each ability can only be attached once.';
    }

    if (this.hasDuplicateDrops()) {
      return 'Each drop item can only be attached once.';
    }

    if (this.dropsArray.length === 0 && Number(this.monsterForm.get('overallDropChance')?.value) > 0) {
      return 'Add at least one drop entry when drop chance is greater than zero.';
    }

    if (this.hasInvalidDrops()) {
      return 'Drop rows must keep quantity above zero, non-negative weights, and at least one positive weight.';
    }

    return 'Review the monster form and try again.';
  }

  private hasInvalidDropChance(): boolean {
    const value = Number(this.monsterForm.get('overallDropChance')?.value);
    return Number.isNaN(value) || value < 0 || value > 1;
  }

  private hasDuplicateStats(): boolean {
    return this.selectedStatKeys.size !== this.statsArray.length;
  }

  private hasDuplicateAbilities(): boolean {
    return this.selectedAbilityIds.size !== this.abilitiesArray.length;
  }

  private hasDuplicateDrops(): boolean {
    return this.selectedDropKeys.size !== this.dropsArray.length;
  }

  private hasInvalidDrops(): boolean {
    let totalWeight = 0;

    for (const control of this.dropsArray.controls) {
      const quantity = Number(control.get('quantity')?.value);
      const weight = Number(control.get('weight')?.value);

      if (Number.isNaN(quantity) || quantity <= 0 || Number.isNaN(weight) || weight < 0) {
        return true;
      }

      totalWeight += weight;
    }

    return this.dropsArray.length > 0 && totalWeight <= 0;
  }

  private getTotalDropWeight(): number {
    return this.dropsArray.controls.reduce((sum, control) => {
      const weight = Number(control.get('weight')?.value);
      return Number.isNaN(weight) || weight < 0 ? sum : sum + weight;
    }, 0);
  }

  private buildSaveRequest(): MonsterSaveRequest | null {
    const name = this.monsterForm.get('name')?.value?.trim();
    if (!name) {
      return null;
    }

    return {
      name,
      overallDropChance: Number(this.monsterForm.get('overallDropChance')?.value),
      stats: this.statsArray.controls.map((control) => ({
        key: control.get('key')?.value as string,
        value: Number(control.get('value')?.value),
      })),
      drops: this.dropsArray.controls.map((control) => ({
        itemTypeName: control.get('itemTypeName')?.value as string,
        itemId: control.get('itemId')?.value as string,
        quantity: Number(control.get('quantity')?.value),
        weight: Number(control.get('weight')?.value),
      })),
      abilityIds: this.abilitiesArray.controls.map((control) => control.get('id')?.value as string),
    };
  }

  private composeDropKey(itemTypeName: string, itemId: string): string {
    return `${itemTypeName}::${itemId}`;
  }

  private clearFormArray(formArray: FormArray): void {
    while (formArray.length > 0) {
      formArray.removeAt(0);
    }
  }
}
