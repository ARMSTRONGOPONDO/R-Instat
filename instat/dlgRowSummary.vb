﻿' R- Instat
' Copyright (C) 2015-2017
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License 
' along with this program.  If not, see <http://www.gnu.org/licenses/>.

Imports instat.Translations
Public Class dlgRowSummary
    Private bFirstLoad As Boolean = True
    Private bReset As Boolean = True
    Private clsDummyRowFunction As New RFunction
    Private clsGetColumnsFunction As New RFunction
    Private clsPipeOperator As New ROperator
    Private clsRowWisePipeOperator As New ROperator
    Private clsRowWiseFunction, clsMutateFunction As New RFunction
    Private clsMeanFunction, clsSumFunction, clsStandardDeviationFunction, clsMinimumFunction, clsMaximumFunction,
    clsMedianFunction, clsCountFunction, clsNumberMissingFunction, clsIsNaFunction, clsIsNotNaFunction, clsAnyDuplicatedFunction,
    clsAnyNaFuction, clsCvFunction, clsGmeanFunction, clsHmeanFunction, clsIQRFunction, clsKurtosisFunction, clsMadFunction, clsMcFunction,
    clsTrimmedMeanFunction, clsMfvFunction, clsMfv1Function, clsQuantileFunction, clsSkewnessFunction, clsRowRanksFunction, clsRowRangesFunction,
    clsRowQuantilesFunction, clsAsMatrixFunction, clsDimensionFunction As New RFunction
    Private clsBaseFunction, clsListFunction As New RFunction

    Private Sub dlgRowSummary_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If bFirstLoad Then
            InitialiseDialog()
            bFirstLoad = False
        End If
        If bReset Then
            SetDefaults()
        End If
        SetRCodeforControls(bReset)
        bReset = False
        autoTranslate(Me)
    End Sub

    Private Sub InitialiseDialog()
        ucrBase.iHelpTopicID = 45

        Dim dctTiesValues As New Dictionary(Of String, String)
        Dim dctProbabilityValues As New Dictionary(Of String, String)
        Dim dctRangeValues As New Dictionary(Of String, String)
        Dim dctTypeValues As New Dictionary(Of String, String)

        ucrReceiverForRowSummaries.SetParameter(New RParameter("x", 0, bNewIncludeArgumentName:=False))
        ucrReceiverForRowSummaries.Selector = ucrSelectorForRowSummaries
        ucrReceiverForRowSummaries.SetMeAsReceiver()
        ucrReceiverForRowSummaries.strSelectorHeading = "Numerics"
        ucrReceiverForRowSummaries.SetIncludedDataTypes({"numeric"})
        ucrReceiverForRowSummaries.bUseFilteredData = False
        ucrReceiverForRowSummaries.bForceAsDataFrame = False
        ucrReceiverForRowSummaries.SetParameterIsString()
        ucrReceiverForRowSummaries.bWithQuotes = False

        ucrChkRowRanks.SetText("Ties")
        ucrChkRowRanks.AddParameterPresentCondition(True, "ties.method", True)
        ucrChkRowRanks.AddParameterPresentCondition(False, "ties.method", False)

        ucrInputRowRanks.SetParameter(New RParameter("ties.method", 2))
        dctTiesValues.Add("average", Chr(34) & "average" & Chr(34))
        dctTiesValues.Add("first", Chr(34) & "first" & Chr(34))
        dctTiesValues.Add("last", Chr(34) & "last" & Chr(34))
        dctTiesValues.Add("max", Chr(34) & "max" & Chr(34))
        dctTiesValues.Add("min", Chr(34) & "min" & Chr(34))
        ucrInputRowRanks.SetItems(dctTiesValues)
        ucrInputRowRanks.bAllowNonConditionValues = True

        'function ran here is probs = c(VALUES)
        ucrInputProbability.SetParameter(New RParameter("p", 1, bNewIncludeArgumentName:=False))
        ucrInputProbability.AddQuotesIfUnrecognised = False
        ucrInputProbability.SetValidationTypeAsNumericList()

        ucrChkType.SetText("Type")
        ucrChkType.AddParameterPresentCondition(True, "type", True)
        ucrChkType.AddParameterPresentCondition(False, "type", False)

        ucrInputType.SetParameter(New RParameter("type", 2))
        dctTypeValues.Add("1", "1")
        dctTypeValues.Add("2", "2")
        dctTypeValues.Add("3", "3")
        dctTypeValues.Add("4", "4")
        dctTypeValues.Add("5", "5")
        dctTypeValues.Add("6", "6")
        dctTypeValues.Add("7", "7")
        dctTypeValues.Add("8", "8")
        dctTypeValues.Add("9", "9")
        ucrInputType.SetItems(dctTypeValues)
        ucrInputType.AddQuotesIfUnrecognised = False
        ucrInputType.bAllowNonConditionValues = True

        ucrChkIgnoreMissingValues.SetParameter(New RParameter("na.rm", 2))
        ucrChkIgnoreMissingValues.SetRDefault("TRUE")
        ucrChkIgnoreMissingValues.bAddRemoveParameter = True
        ucrChkIgnoreMissingValues.bChangeParameterValue = False
        ucrChkIgnoreMissingValues.SetText("Ignore Missing Values")

        'linking controls
        ucrPnlRowSummaries.AddRadioButton(rdoSingle)
        ucrPnlRowSummaries.AddRadioButton(rdoMultiple)

        ucrPnlRowSummaries.AddFunctionNamesCondition(rdoSingle, {"rowwise"}, False)
        ucrPnlRowSummaries.AddFunctionNamesCondition(rdoMultiple, {"rowRanks", "rowRanges", "rowQuantiles"})

        ucrPnlStatistics.AddRadioButton(rdoMean)
        ucrPnlStatistics.AddRadioButton(rdoMinimum)
        ucrPnlStatistics.AddRadioButton(rdoSum)
        ucrPnlStatistics.AddRadioButton(rdoMedian)
        ucrPnlStatistics.AddRadioButton(rdoNumberMissing)
        ucrPnlStatistics.AddRadioButton(rdoStandardDeviation)
        ucrPnlStatistics.AddRadioButton(rdoMaximum)
        ucrPnlStatistics.AddRadioButton(rdoCount)
        ucrPnlStatistics.AddRadioButton(rdoMore)

        ucrPnlStatistics.AddFunctionNamesCondition(rdoMean, "Mean", False)
        ucrPnlStatistics.AddFunctionNamesCondition(rdoMinimum, "Minimum")
        ucrPnlStatistics.AddFunctionNamesCondition(rdoSum, "Sum")
        ucrPnlStatistics.AddFunctionNamesCondition(rdoMedian, "Median")
        ucrPnlStatistics.AddFunctionNamesCondition(rdoNumberMissing, "Number_missing")
        ucrPnlStatistics.AddFunctionNamesCondition(rdoStandardDeviation, "Standard_deviation")
        ucrPnlStatistics.AddFunctionNamesCondition(rdoMaximum, "Maximum")
        ucrPnlStatistics.AddFunctionNamesCondition(rdoCount, "Count")
        ucrPnlStatistics.AddFunctionNamesCondition(rdoMore, {"anyDuplicated", "anyNA", "cv", "Gmean", "Hmean", "IQR", "kurtosis",
                                                   "mad", "mc", "mean, trim=0.2", "mfv1", "quantile, probs=0.5", "skewness"})

        ucrPnlMultipleRowSummary.AddRadioButton(rdoRowRanks)
        ucrPnlMultipleRowSummary.AddRadioButton(rdoRowRange)
        ucrPnlMultipleRowSummary.AddRadioButton(rdoRowQuantile)
        ucrPnlMultipleRowSummary.AddParameterValuesCondition(rdoRowRanks, "check", "rowRanks")
        ucrPnlMultipleRowSummary.AddParameterValuesCondition(rdoRowRange, "check", "rowRange")
        ucrPnlMultipleRowSummary.AddParameterValuesCondition(rdoRowQuantile, "check", "rowQuantiles")

        ucrPnlStatistics.AddToLinkedControls(ucrChkIgnoreMissingValues, {rdoMean, rdoMinimum, rdoSum, rdoMedian, rdoStandardDeviation, rdoMaximum}, bNewLinkedHideIfParameterMissing:=True)
        ucrPnlStatistics.AddToLinkedControls(ucrInputUserDefined, {rdoMore}, bNewLinkedHideIfParameterMissing:=True)
        ucrPnlMultipleRowSummary.AddToLinkedControls(ucrChkRowRanks, {rdoRowRanks}, bNewLinkedHideIfParameterMissing:=True)
        ucrPnlMultipleRowSummary.AddToLinkedControls({ucrInputProbability, ucrChkType}, {rdoRowQuantile}, bNewLinkedHideIfParameterMissing:=True)
        ucrPnlRowSummaries.AddToLinkedControls(ucrPnlStatistics, {rdoSingle}, bNewLinkedHideIfParameterMissing:=True, bNewLinkedAddRemoveParameter:=True)
        ucrPnlRowSummaries.AddToLinkedControls(ucrPnlMultipleRowSummary, {rdoMultiple}, bNewLinkedHideIfParameterMissing:=True)

        ucrPnlStatistics.SetLinkedDisplayControl(grpStatistic)
        ucrPnlMultipleRowSummary.SetLinkedDisplayControl(grpMultipleRowSummary)

        ucrChkRowRanks.AddToLinkedControls({ucrInputRowRanks}, {True}, bNewLinkedHideIfParameterMissing:=True, bNewLinkedAddRemoveParameter:=True,
                           bNewLinkedUpdateFunction:=True, bNewLinkedChangeToDefaultState:=True, objNewDefaultState:="average")
        ucrChkType.AddToLinkedControls({ucrInputType}, {True}, bNewLinkedHideIfParameterMissing:=True, bNewLinkedAddRemoveParameter:=True,
                                        bNewLinkedUpdateFunction:=True, bNewLinkedChangeToDefaultState:=True, objNewDefaultState:="7")
        'ucrInputUserDefined
        ucrInputUserDefined.SetItems({"anyDuplicated", "anyNA", "cv", "Gmean", "Hmean", "IQR", "kurtosis", "mad", "mc", "mean, trim=0.2",
                                     "mfv1", "quantile, probs=0.5", "skewness"})

        ucrNewDataFrameName.SetPrefix("row_summary")
        ucrNewDataFrameName.SetSaveTypeAsDataFrame()
        ucrNewDataFrameName.SetLabelText("New Dataframe Name:")
        ucrNewDataFrameName.SetIsTextBox()

        ucrSaveNewDataFrame.SetLabelText("New column name:")
        ucrSaveNewDataFrame.SetSaveTypeAsColumn()
        ucrSaveNewDataFrame.SetIsComboBox()
        ucrSaveNewDataFrame.SetPrefix("row_summary")
        ucrSaveNewDataFrame.SetDataFrameSelector(ucrSelectorForRowSummaries.ucrAvailableDataFrames)
    End Sub

    Private Sub SetDefaults()
        clsPipeOperator = New ROperator
        clsRowWisePipeOperator = New ROperator
        clsDummyRowFunction = New RFunction
        clsGetColumnsFunction = New RFunction
        clsRowWiseFunction = New RFunction
        clsMutateFunction = New RFunction
        clsBaseFunction = New RFunction
        clsListFunction = New RFunction
        clsMeanFunction = New RFunction
        clsSumFunction = New RFunction
        clsStandardDeviationFunction = New RFunction
        clsMinimumFunction = New RFunction
        clsMaximumFunction = New RFunction
        clsMedianFunction = New RFunction
        clsCountFunction = New RFunction
        clsIsNaFunction = New RFunction
        clsIsNotNaFunction = New RFunction
        clsNumberMissingFunction = New RFunction
        clsAnyDuplicatedFunction = New RFunction
        clsAnyNaFuction = New RFunction
        clsCvFunction = New RFunction
        clsGmeanFunction = New RFunction
        clsHmeanFunction = New RFunction
        clsIQRFunction = New RFunction
        clsKurtosisFunction = New RFunction
        clsMadFunction = New RFunction
        clsMcFunction = New RFunction
        clsTrimmedMeanFunction = New RFunction
        clsMfv1Function = New RFunction
        clsMfvFunction = New RFunction
        clsQuantileFunction = New RFunction
        clsSkewnessFunction = New RFunction
        clsRowRanksFunction = New RFunction
        clsRowRangesFunction = New RFunction
        clsRowQuantilesFunction = New RFunction
        clsAsMatrixFunction = New RFunction
        clsDimensionFunction = New RFunction

        'reset
        ucrSelectorForRowSummaries.Reset()
        ucrReceiverForRowSummaries.SetMeAsReceiver()
        ucrNewDataFrameName.Reset()
        ucrInputProbability.Reset()
        ucrInputUserDefined.SetName("anyDuplicated")

        clsDummyRowFunction.AddParameter("check", "rowRanks", iPosition:=0)

        clsPipeOperator.SetOperation("%>%")
        clsPipeOperator.AddParameter("left", clsRFunctionParameter:=ucrSelectorForRowSummaries.ucrAvailableDataFrames.clsCurrDataFrame, iPosition:=0)
        clsPipeOperator.AddParameter("right", clsROperatorParameter:=clsRowWisePipeOperator, iPosition:=1)

        clsRowWiseFunction.SetPackageName("dplyr")
        clsRowWiseFunction.SetRCommand("rowwise")

        clsMutateFunction.SetPackageName("dplyr")
        clsMutateFunction.SetRCommand("mutate")
        clsMutateFunction.AddParameter("Mean", clsRFunctionParameter:=clsMeanFunction, iPosition:=0)

        clsRowWisePipeOperator.SetOperation("%>%")
        clsRowWisePipeOperator.AddParameter("left", clsRFunctionParameter:=clsRowWiseFunction, iPosition:=0)
        clsRowWisePipeOperator.AddParameter("right", clsRFunctionParameter:=clsMutateFunction, iPosition:=1)

        clsMeanFunction.SetRCommand("mean")
        clsSumFunction.SetRCommand("sum")
        clsStandardDeviationFunction.SetRCommand("sd")
        clsMinimumFunction.SetRCommand("min")
        clsMaximumFunction.SetRCommand("max")
        clsMedianFunction.SetRCommand("median")
        clsAnyDuplicatedFunction.SetRCommand("anyDuplicated")
        clsAnyNaFuction.SetRCommand("anyNA")
        clsCvFunction.SetPackageName("raster")
        clsCvFunction.SetRCommand("cv")
        clsGmeanFunction.SetPackageName("DescTools")
        clsGmeanFunction.SetRCommand("Gmean")
        clsHmeanFunction.SetPackageName("DescTools")
        clsHmeanFunction.SetRCommand("Hmean")
        clsIQRFunction.SetRCommand("IQR")
        clsKurtosisFunction.SetPackageName("e1071")
        clsKurtosisFunction.SetRCommand("kurtosis")
        clsMadFunction.SetRCommand("mad")
        clsMcFunction.SetPackageName("robustbase")
        clsMcFunction.SetRCommand("mc")
        clsTrimmedMeanFunction.SetRCommand("mean")
        clsTrimmedMeanFunction.AddParameter("trim", "0.2", iPosition:=1)
        clsMfv1Function.SetPackageName("statip")
        clsMfv1Function.SetRCommand("mfv1")
        clsQuantileFunction.SetRCommand("quantile")
        clsQuantileFunction.AddParameter("probs", "0.5", iPosition:=1)
        clsSkewnessFunction.SetPackageName("e1071")
        clsSkewnessFunction.SetRCommand("skewness")

        clsIsNaFunction.SetRCommand("is.na")
        clsIsNotNaFunction.SetRCommand("!is.na")

        clsBaseFunction.SetRCommand("data_book$import_data")
        clsBaseFunction.AddParameter("data_tables", clsRFunctionParameter:=clsListFunction)

        clsListFunction.SetRCommand("list")

        clsRowRanksFunction.SetPackageName("matrixStats")
        clsRowRanksFunction.SetRCommand("rowRanks")
        clsRowRanksFunction.AddParameter("x", clsRFunctionParameter:=clsAsMatrixFunction, iPosition:=0, bIncludeArgumentName:=False)
        clsRowRanksFunction.AddParameter("dim.", clsRFunctionParameter:=clsDimensionFunction, iPosition:=1)

        clsRowRangesFunction.SetPackageName("matrixStats")
        clsRowRangesFunction.SetRCommand("rowRanges")
        clsRowRangesFunction.AddParameter("dim.", clsRFunctionParameter:=clsDimensionFunction, iPosition:=1)
        clsRowRangesFunction.AddParameter("x", clsRFunctionParameter:=clsAsMatrixFunction, iPosition:=0, bIncludeArgumentName:=False)

        clsDimensionFunction.SetRCommand("dim")

        clsRowQuantilesFunction.SetPackageName("matrixStats")
        clsRowQuantilesFunction.SetRCommand("rowQuantiles")
        clsRowQuantilesFunction.AddParameter("x", clsRFunctionParameter:=clsAsMatrixFunction, iPosition:=0, bIncludeArgumentName:=False)

        clsAsMatrixFunction.SetRCommand("as.matrix")

        ucrBase.clsRsyntax.SetBaseRFunction(clsBaseFunction)
    End Sub

    Private Sub SetRCodeforControls(bReset As Boolean)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsSumFunction, ucrReceiverForRowSummaries.GetParameter(), iAdditionalPairNo:=1)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsStandardDeviationFunction, ucrReceiverForRowSummaries.GetParameter(), iAdditionalPairNo:=2)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsMinimumFunction, ucrReceiverForRowSummaries.GetParameter(), iAdditionalPairNo:=3)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsMaximumFunction, ucrReceiverForRowSummaries.GetParameter(), iAdditionalPairNo:=4)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsMedianFunction, ucrReceiverForRowSummaries.GetParameter(), iAdditionalPairNo:=5)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsIsNotNaFunction, New RParameter("x", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=6)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsIsNaFunction, New RParameter("x", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=7)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsAnyDuplicatedFunction, New RParameter("anyDuplicated", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=8)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsAnyNaFuction, New RParameter("anyNA", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=9)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsCvFunction, New RParameter("cv", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=10)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsGmeanFunction, New RParameter("Gmean", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=11)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsHmeanFunction, New RParameter("Hmean", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=12)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsIQRFunction, New RParameter("IQR", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=13)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsKurtosisFunction, New RParameter("kurtosis", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=14)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsMadFunction, New RParameter("mad", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=15)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsMcFunction, New RParameter("mc", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=16)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsTrimmedMeanFunction, New RParameter("x", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=17)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsMfv1Function, New RParameter("mfv1", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=18)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsQuantileFunction, New RParameter("x", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=19)
        ucrReceiverForRowSummaries.AddAdditionalCodeParameterPair(clsSkewnessFunction, New RParameter("skewness", 0, bNewIncludeArgumentName:=False), iAdditionalPairNo:=20)
        ucrChkIgnoreMissingValues.AddAdditionalCodeParameterPair(clsSumFunction, ucrChkIgnoreMissingValues.GetParameter(), iAdditionalPairNo:=1)
        ucrChkIgnoreMissingValues.AddAdditionalCodeParameterPair(clsStandardDeviationFunction, ucrChkIgnoreMissingValues.GetParameter(), iAdditionalPairNo:=2)
        ucrChkIgnoreMissingValues.AddAdditionalCodeParameterPair(clsMinimumFunction, ucrChkIgnoreMissingValues.GetParameter(), iAdditionalPairNo:=3)
        ucrChkIgnoreMissingValues.AddAdditionalCodeParameterPair(clsMaximumFunction, ucrChkIgnoreMissingValues.GetParameter(), iAdditionalPairNo:=4)
        ucrChkIgnoreMissingValues.AddAdditionalCodeParameterPair(clsMedianFunction, ucrChkIgnoreMissingValues.GetParameter(), iAdditionalPairNo:=5)
        ucrSaveNewDataFrame.AddAdditionalRCode(clsRowRangesFunction, iAdditionalPairNo:=1)
        ucrSaveNewDataFrame.AddAdditionalRCode(clsRowQuantilesFunction, iAdditionalPairNo:=2)
        ucrChkRowRanks.SetRCode(clsRowRanksFunction, bReset)
        ucrChkType.SetRCode(clsRowQuantilesFunction, bReset)
        ucrChkIgnoreMissingValues.SetRCode(clsMeanFunction, bReset)
        ucrReceiverForRowSummaries.SetRCode(clsMeanFunction, bReset)
        ucrPnlMultipleRowSummary.SetRCode(clsDummyRowFunction, bReset)
        ucrSaveNewDataFrame.SetRCode(clsRowRanksFunction, bReset)
        If bReset Then
            ucrPnlStatistics.SetRCode(clsMeanFunction, bReset)
            ucrPnlRowSummaries.SetRCode(clsBaseFunction, bReset)
        End If
    End Sub

    Private Sub TestOKEnabled()
        If Not ucrReceiverForRowSummaries.IsEmpty AndAlso ucrNewDataFrameName.IsComplete Then
            ucrBase.OKEnabled(True)
        Else
            ucrBase.OKEnabled(False)
        End If
    End Sub

    Private Sub ucrBase_ClickReset(sender As Object, e As EventArgs) Handles ucrBase.ClickReset
        SetDefaults()
        SetRCodeforControls(True)
        TestOKEnabled()
    End Sub

    Private Sub ucrPnlRowSummaries_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrPnlRowSummaries.ControlValueChanged, ucrPnlStatistics.ControlValueChanged, ucrInputUserDefined.ControlValueChanged, ucrPnlMultipleRowSummary.ControlValueChanged
        If rdoSingle.Checked Then
            ucrReceiverForRowSummaries.SetMeAsReceiver()
            clsMutateFunction.RemoveParameterByName("Mean")
            clsMutateFunction.RemoveParameterByName("Sum")
            clsMutateFunction.RemoveParameterByName("Standard_deviation")
            clsMutateFunction.RemoveParameterByName("Minimum")
            clsMutateFunction.RemoveParameterByName("Maximum")
            clsMutateFunction.RemoveParameterByName("Median")
            clsMutateFunction.RemoveParameterByName("Count")
            clsMutateFunction.RemoveParameterByName("Number_missing")
            clsMutateFunction.RemoveParameterByName("anyDuplicated")
            clsMutateFunction.RemoveParameterByName("anyNA")
            clsMutateFunction.RemoveParameterByName("cv")
            clsMutateFunction.RemoveParameterByName("Gmean")
            clsMutateFunction.RemoveParameterByName("Hmean")
            clsMutateFunction.RemoveParameterByName("IQR")
            clsMutateFunction.RemoveParameterByName("kurtosis")
            clsMutateFunction.RemoveParameterByName("mad")
            clsMutateFunction.RemoveParameterByName("mc")
            clsMutateFunction.RemoveParameterByName("mean")
            clsMutateFunction.RemoveParameterByName("mfv1")
            clsMutateFunction.RemoveParameterByName("quantile")
            clsMutateFunction.RemoveParameterByName("skewness")
            clsSumFunction.RemoveParameterByName("x")
            clsListFunction.ClearParameters()
            If rdoMean.Checked Then
                clsMutateFunction.AddParameter("Mean", clsRFunctionParameter:=clsMeanFunction, iPosition:=0)
            ElseIf rdoSum.Checked Then
                clsSumFunction.AddParameter("x", ucrReceiverForRowSummaries.GetVariableNames("False"), bIncludeArgumentName:=False, iPosition:=0)
                clsMutateFunction.AddParameter("Sum", clsRFunctionParameter:=clsSumFunction, iPosition:=0)
            ElseIf rdoStandardDeviation.Checked Then
                clsMutateFunction.AddParameter("Standard_deviation", clsRFunctionParameter:=clsStandardDeviationFunction, iPosition:=0)
            ElseIf rdoMinimum.Checked Then
                clsMutateFunction.AddParameter("Minimum", clsRFunctionParameter:=clsMinimumFunction, iPosition:=0)
            ElseIf rdoMaximum.Checked Then
                clsMutateFunction.AddParameter("Maximum", clsRFunctionParameter:=clsMaximumFunction, iPosition:=0)
            ElseIf rdoMedian.Checked Then
                clsMutateFunction.AddParameter("Median", clsRFunctionParameter:=clsMedianFunction, iPosition:=0)
            ElseIf rdoCount.Checked Then
                clsSumFunction.AddParameter("x", clsRFunctionParameter:=clsIsNotNaFunction, iPosition:=0, bIncludeArgumentName:=False)
                clsMutateFunction.AddParameter("Count", clsRFunctionParameter:=clsSumFunction, iPosition:=0)
            ElseIf rdoNumberMissing.Checked Then
                clsSumFunction.AddParameter("x", clsRFunctionParameter:=clsIsNaFunction, iPosition:=0, bIncludeArgumentName:=False)
                clsMutateFunction.AddParameter("Number_missing", clsRFunctionParameter:=clsSumFunction, iPosition:=0)
            ElseIf rdoMore.Checked Then
                Select Case ucrInputUserDefined.GetText()
                    Case "anyDuplicated"
                        clsMutateFunction.AddParameter("anyDuplicated", clsRFunctionParameter:=clsAnyDuplicatedFunction, iPosition:=0)
                    Case "anyNA"
                        clsMutateFunction.AddParameter("anyNA", clsRFunctionParameter:=clsAnyNaFuction, iPosition:=0)
                    Case "cv"
                        clsMutateFunction.AddParameter("cv", clsRFunctionParameter:=clsCvFunction, iPosition:=0)
                    Case "Gmean"
                        clsMutateFunction.AddParameter("Gmean", clsRFunctionParameter:=clsGmeanFunction, iPosition:=0)
                    Case "Hmean"
                        clsMutateFunction.AddParameter("Hmean", clsRFunctionParameter:=clsHmeanFunction, iPosition:=0)
                    Case "IQR"
                        clsMutateFunction.AddParameter("IQR", clsRFunctionParameter:=clsIQRFunction, iPosition:=0)
                    Case "kurtosis"
                        clsMutateFunction.AddParameter("kurtosis", clsRFunctionParameter:=clsKurtosisFunction, iPosition:=0)
                    Case "mad"
                        clsMutateFunction.AddParameter("mad", clsRFunctionParameter:=clsMadFunction, iPosition:=0)
                    Case "mc"
                        clsMutateFunction.AddParameter("mc", clsRFunctionParameter:=clsMcFunction, iPosition:=0)
                    Case "mean, trim=0.2"
                        clsMutateFunction.AddParameter("mean", clsRFunctionParameter:=clsTrimmedMeanFunction, bIncludeArgumentName:=False, iPosition:=0)
                    Case "mfv1"
                        clsMutateFunction.AddParameter("mfv1", clsRFunctionParameter:=clsMfv1Function, iPosition:=0)
                    Case "quantile, probs=0.5"
                        clsMutateFunction.AddParameter("quantile", clsRFunctionParameter:=clsQuantileFunction, bIncludeArgumentName:=False, iPosition:=0)
                    Case "skewness"
                        clsMutateFunction.AddParameter("skewness", clsRFunctionParameter:=clsSkewnessFunction, iPosition:=0)
                End Select
            End If
            clsPipeOperator.SetAssignTo(ucrNewDataFrameName.GetText)
            clsListFunction.AddParameter(ucrNewDataFrameName.GetText, clsROperatorParameter:=clsPipeOperator, iPosition:=0)
            ucrBase.clsRsyntax.SetBaseRFunction(clsBaseFunction)
        Else
            If rdoRowRange.Checked Then
                clsDummyRowFunction.AddParameter("check", "rowRange", iPosition:=0)
                ucrBase.clsRsyntax.SetBaseRFunction(clsRowRangesFunction)
            ElseIf rdoRowQuantile.Checked Then
                clsDummyRowFunction.AddParameter("check", "rowQuantiles", iPosition:=0)
                ucrBase.clsRsyntax.SetBaseRFunction(clsRowQuantilesFunction)
            Else
                clsDummyRowFunction.AddParameter("check", "rowRanks", iPosition:=0)
                ucrBase.clsRsyntax.SetBaseRFunction(clsRowRanksFunction)
            End If
        End If

        If rdoMultiple.Checked AndAlso rdoRowQuantile.Checked Then
            clsDummyRowFunction.AddParameter("0", clsRFunctionParameter:=clsRowQuantilesFunction, iPosition:=0)
            ucrInputProbability.SetName("0.25,0.5,0.75")
            ucrInputProbability.SetItems({"0.25,0.5,0.75", "0, 0.2, 0.4, 0.6, 0.8, 1 ", "0.5, 0.8, 1"})
        End If
    End Sub

    Private Sub ucrReceiverForRowSummaries_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrReceiverForRowSummaries.ControlValueChanged
        Dim clsGetColumnsFunction As New RFunction
        clsGetColumnsFunction = ucrReceiverForRowSummaries.GetVariables()
        clsGetColumnsFunction.SetAssignTo("columns")
        clsAsMatrixFunction.AddParameter("columns", clsRFunctionParameter:=clsGetColumnsFunction, iPosition:=0, bIncludeArgumentName:=False)
        clsDimensionFunction.AddParameter("columns", clsRFunctionParameter:=clsGetColumnsFunction, iPosition:=0, bIncludeArgumentName:=False)
    End Sub

    Private Sub ucrInputProbability_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrInputProbability.ControlValueChanged
        clsRowQuantilesFunction.AddParameter("probs", "c(" & ucrInputProbability.GetText & ")", iPosition:=1)
    End Sub

    Private Sub Controls_ControlContentsChanged(ucrChangedControl As ucrCore) Handles ucrReceiverForRowSummaries.ControlContentsChanged, ucrPnlStatistics.ControlContentsChanged, ucrPnlMultipleRowSummary.ControlContentsChanged, ucrPnlRowSummaries.ControlContentsChanged, ucrChkRowRanks.ControlContentsChanged,
        ucrChkType.ControlContentsChanged, ucrInputProbability.ControlContentsChanged, ucrInputRowRanks.ControlContentsChanged, ucrInputType.ControlContentsChanged
        TestOKEnabled()
    End Sub
End Class