<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:decimal-format name="Infinity-format" NaN="" infinity="-" />
	<xsl:template match="/">
		<div id="tableContainer">
			<xsl:choose>
				<xsl:when test ="count(//row[@Element='Op']) &gt;0">
					<xsl:for-each select="//row[@Element='Pso']">	
						<xsl:variable name="PSOFKID" select="@PSOID" />
						<table  align="center" id="tblmain" width="100%" cellspacing="1" cellpadding="0"  class = "inner_tb" border="1" >
							<tr class="tableContent1" id = "Town" align="left">
								<th width = "10%">Name </th>
								<td  colSpan="6"  id = "idPSO" width = "25%">
								<xsl:value-of select="@EmpName" />
								</td>
								<th  width = "10%">Territory</th>
								<td  colSpan="6"  id = "idTN" width = "25%">
								<xsl:value-of select="@TerName" />
								</td>
								
								<th  width = "25%">No.of MCL Doctors</th>
								<td class = "tb-r1" colSpan="6"  id = "idmcl" width = "25%">
								<xsl:value-of select="@MClCount" />
								</td>
							</tr>
						</table>
						<table align="center" width="100%" cellspacing="1" cellpadding="0" class="inner_tb"  border="1">
							<tr align="center">
								<th rowspan="2">Product</th>
								<th rowspan="2">Sample Pack</th>
								<th rowspan="2">Last Acknowledgement Date</th>
								<th colspan="5" align="center">Samples</th>
							</tr>
							<tr align="center">
								<th>Allocated Quantity</th>	
								<th>Opening Balance</th>				
								<th>
									<a class="link1mod">
                    Received
                    <xsl:attribute name="href">
											javascript:showReport('0','<xsl:value-of select="@PsoFKID"/>','<xsl:value-of select="@TerId"/>')
										</xsl:attribute>
										
									</a>
								</th>
								<th>
									<a class="link1mod">
                    Disbursed
                    <xsl:attribute name="href">
											javascript:showReportDisbursed('0','<xsl:value-of select="@PsoFKID"/>','<xsl:value-of select="@TerId"/>')
										</xsl:attribute>
										
									</a>
								</th>
								<th>Closing Balance</th>
							</tr>
							<xsl:apply-templates select="//row[@Element='Op' and @PsoFKID=$PSOFKID]" />				
						</table><br></br> 
					</xsl:for-each>								
				</xsl:when>
				<xsl:otherwise>
					<table align="center" width="100%" cellspacing="1" class = "inner_tb" cellpadding="0"  border="1">
						<tr class="tableContent1" align="center">
							<th  rowspan="2">Product</th>
							<th  rowspan="2">Sample Pack</th>
							<th  rowspan="2">Last Acknowledgement Date</th>
							<th  colspan="5">Samples</th>
						</tr>
						<tr class="tableContent1" align="center">
							<th>Allocated Quantity</th>
							<th>Opening Balance</th>				
							<th>Received</th>
							<th>Disbursed</th>
							<th>Closing Balance</th>
						</tr>
						<tr class="tablecontent1" align="center"> 
							<td align="center" class="alerttext" colspan="10" >No Records Found</td>
						</tr>
					</table>
				</xsl:otherwise>
			</xsl:choose>
		</div>
	</xsl:template>

	<xsl:template match="row[@Element='Op']">
		<tr class="tb-r1">
			<td align="left">
				<input type="hidden" id="hinpkid" >
					<xsl:attribute name="value">
						<xsl:value-of select="@SampleFKID" />
					</xsl:attribute>
				</input>
				<xsl:value-of select="@ProductName" />
			</td>
			<td align="left">
				<xsl:value-of select="@SamplePack" />
			</td>
			<td align="left">
				<xsl:value-of select="@ackDate" />
			</td>
			<td align="left">
				<xsl:value-of select="@AllocatedQuantity" />
			</td>
			<td align="left">
				<xsl:value-of select="@OpeningBalance" />
			</td>
			<td align="left">
				<a class="link1" >
					<xsl:attribute name="href">
						javascript:showReport('<xsl:value-of select="@SampleFKID"/>','<xsl:value-of select="@PsoFKID"/>','<xsl:value-of select="@TerId"/>')
					</xsl:attribute>
					<xsl:value-of select="@Received" />
				</a>
			</td>
			<td align="left">
				<a class="link1" >
					<xsl:attribute name="href">
						javascript:showReportDisbursed('<xsl:value-of select="@SampleFKID"/>','<xsl:value-of select="@PsoFKID"/>','<xsl:value-of select="@TerId"/>')
					</xsl:attribute>
					<xsl:value-of select="@Disbursed" />
				</a>
			</td>
			<td align="left">
				<xsl:value-of select="@ClosingBalance" />
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>