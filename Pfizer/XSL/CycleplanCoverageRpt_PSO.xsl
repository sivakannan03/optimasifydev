<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:decimal-format name="Infinity-format" NaN="0" infinity="-" />

<xsl:template match="/">
	<div id="tableContainer">
		<table align="center" width="100%" cellspacing="1" cellpadding="0" class="inner_tb"  border="1">
			<tr  align="center">
				<th  colspan="11">Doctor Call Average</th>
				<th  colspan="5">Chemist Call Average</th>
				<th  colspan="4">Retailer Blitz</th>
			</tr>
			
			<tr  align="center">
				<td class="tb-r2" width="5%">Freq</td>
				<td class="tb-r2" width="5%">Plan Visit</td>
				<td class="tb-r2" width="5%">V0</td>
				<td class="tb-r2" width="5%">V1</td>				
				<td class="tb-r2" width="5%">V2</td>
				<td class="tb-r2" width="5%">V3</td>
				<td class="tb-r2" width="5%">Cov %</td>
				<td class="tb-r2" width="5%">Fre %</td>
				<td class="tb-r2" width="6%">Days Worked</td>
				<td class="tb-r2" width="6%">Repeat Calls</td>
				<td class="tb-r2" width="6%">Call Avg</td>
								
				<td class="tb-r2" width="6%">No.Of Mcl</td>
				<td class="tb-r2" width="6%">Met</td>				
				<td class="tb-r2" width="6%">Repeat Calls</td>	
				<td class="tb-r2" width="6%">Call Avg</td>
				<td class="tb-r2" width="6%">Cov %</td>
				
				<td class="tb-r2" width="6%">Blitz Days</td>
				<td class="tb-r2" width="6%">Met</td>
				<td class="tb-r2" width="6%">Repeat Calls</td>	
				<td class="tb-r2" width="6%">Call avg</td>
			</tr>
			<xsl:choose>
				<xsl:when test="count(//row[@Element='Cycle']) &gt;0">
					<xsl:apply-templates select="//row[@Element='Cycle']" />
					<tr  align="left">
						<th align="right" >Total</th>
						<td align="right" class="tb-r1"><xsl:value-of select="sum(//row[@Element='Cycle']/@TPVisit)" /></td>
						<td align="right" class="tb-r1"><xsl:value-of select="sum(//row[@Element='Cycle']/@v0)" /></td>
						<td align="right" class="tb-r1"><xsl:value-of select="sum(//row[@Element='Cycle']/@v1)" /></td>
						
						
						
						<!--<td align="right" class="heading2"><xsl:value-of select="format-number(sum(//row[@Element='Cycle']/@v2),'##,##,###','Infinity-format')" /></td>
						<td align="right" class="heading2"><xsl:value-of select="format-number(sum(//row[@Element='Cycle']/@v3),'##,##,###','Infinity-format')" /></td>-->
						
						<td align="right" class="heading2"><xsl:value-of select="sum(//row[@Element='Cycle']/@v2)*2" /></td>
						<td align="right" class="heading2"><xsl:value-of select="sum(//row[@Element='Cycle']/@v3)*3" /></td>	
						
						
						<!--<td align="right"><xsl:value-of select="format-number(sum(//row[@Element='Cycle']/@cyper),'##,##,###.##','Infinity-format')" /></td>	
						<td align="right"><xsl:value-of select="format-number(sum(//row[@Element='Cycle']/@Freper),'##,##,###.##','Infinity-format')" /></td>-->					
						
						<td align="right" class="heading2">
							<!--<xsl:value-of select="format-number(((sum(//row[@Element='Cycle']/@v1)+sum(//row[@Element='Cycle']/@v2)+sum(//row[@Element='Cycle']/@v3)) div sum(//row[@Element='Cycle']/@PlanVisit))*100,'##,##,###.##','Infinity-format')" />-->
							<xsl:value-of select="format-number((sum(//row[@Element='Cycle']/@AVisits) div sum(//row[@Element='Cycle']/@TPVisit))*100,'##,##,###.##','Infinity-format')" />
						</td>	
						<td align="right" class="heading2">
							<xsl:value-of select="format-number((sum(//row[@Element='Cycle']/@FreVal) div sum(//row[@Element='Cycle']/@PlanVisit))*100,'##,##,###.##','Infinity-format')" />
						</td>	
					</tr>				
				</xsl:when>
				<xsl:otherwise>
					<tr class="tablecontent1">
						<td align="center" class="alerttext" colspan="21">No Records Found</td>
					</tr>
				</xsl:otherwise>
			</xsl:choose>
		</table>
	</div>
</xsl:template>
<xsl:template match="row[@Element='Cycle']">
	<tr class="tablecontent1">
		<td align="right">					
			<xsl:value-of select="@Fre" />
		</td>
		<td align="right">
			<xsl:value-of select="@PlanVisit" />
		</td>
		<td align="right">
			<xsl:value-of select="@v0" />
		</td>
		<td align="right">
			<xsl:value-of select="@v1" />
		</td>
		<td align="right">
			<xsl:value-of select="@v2" />
		</td>
		<td align="right">
			<xsl:value-of select="@v3" />
		</td>
		
		<td align="right">
			<!--<xsl:value-of select="@cyper" />-->
			<xsl:value-of select="format-number((@AVisits div @TPVisit)*100,'##,##,###.##','Infinity-format')" />
		</td>
		<td align="right">
			<xsl:if test="@Fre='V3'">
				<xsl:value-of select="format-number((@v3 div @PlanVisit)*100,'##,##,###.##','Infinity-format')" />
			</xsl:if>
			<xsl:if test="@Fre='V2'">
				<xsl:value-of select="format-number((@v2 div @PlanVisit)*100,'##,##,###.##','Infinity-format')" />
			</xsl:if>
			<xsl:if test="@Fre='V1'">
				<xsl:value-of select="format-number((@v1 div @PlanVisit)*100,'##,##,###.##','Infinity-format')" />
			</xsl:if>
			<!--<xsl:value-of select="@Freper" />-->
		</td>
		<xsl:if test ="preceding-sibling::*[1]/@NoDWork != @NoDWork or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@NoDWork" />
			</td>
		</xsl:if>
		<xsl:if test ="preceding-sibling::*[1]/@ReC != @ReC or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@ReC" />
			</td>
		</xsl:if>
		<xsl:if test ="preceding-sibling::*[1]/@DCA != @DCA or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@DCA" />
				<!--<xsl:value-of select="format-number(((//row[@Element='Cycle_AV']/@AVisits+@ReC) div @NoDWork),'##,##,###.##','Infinity-format')" />-->
			</td>
		</xsl:if>
		<xsl:if test ="preceding-sibling::*[1]/@ChMcl != @ChMcl or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@ChMcl" />
			</td>
		</xsl:if>
		<xsl:if test ="preceding-sibling::*[1]/@Chmet != @Chmet or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@Chmet" />
			</td>
		</xsl:if>
		
		<!-- Added by Raga on 18th Jan 2011!-->
		<xsl:if test ="preceding-sibling::*[1]/@CReC != @CReC or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@CReC" />
			</td>
		</xsl:if>
		<xsl:if test ="preceding-sibling::*[1]/@Chcaav != @Chcaav or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@Chcaav" />
			</td>
		</xsl:if>
		<xsl:if test ="preceding-sibling::*[1]/@ChCovper != @ChCovper or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@ChCovper" />
			</td>
		</xsl:if>
		
		
		
		<xsl:if test ="preceding-sibling::*[1]/@noRe != @noRe or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@noRe" />
			</td>
		</xsl:if>
		<xsl:if test ="preceding-sibling::*[1]/@Rmet != @Rmet or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@Rmet" />
			</td>
		</xsl:if>
		
		<!--Added by Raga on 18th Jan 2011 !-->
		<xsl:if test ="preceding-sibling::*[1]/@RReC != @RReC or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@RReC" />
			</td>
		</xsl:if>
		
		<xsl:if test ="preceding-sibling::*[1]/@ChcaavR != @ChcaavR or position() = '1'">					
			<td align="center">
				<xsl:attribute name="rowspan">
					<xsl:value-of select="count(//row[@Element='Cycle'])+1"></xsl:value-of>	
				</xsl:attribute>
				<xsl:value-of select="@ChcaavR" />
			</td>
		</xsl:if>
		
		
		<!--<td align="left">
			<xsl:value-of select="@NoDWork" />
		</td>
		<td align="left">
			<xsl:value-of select="@ReC" />
		</td>
		<td align="left">
			<xsl:value-of select="@DCA" />
		</td>
		<td align="left">
			<xsl:value-of select="@ChMcl" />
		</td>
		
		<td align="left">
			<xsl:value-of select="@Chmet" />
		</td>
		<td align="left">
			<xsl:value-of select="@ChCovper" />
		</td>
		<td align="left">
			<xsl:value-of select="@Chcaav" />
		</td>
		<td align="left">
			<xsl:value-of select="@noRe" />
		</td>
		<td align="left">
			<xsl:value-of select="@Rmet" />
		</td>
		<td align="left">
			<xsl:value-of select="@ChcaavR" />
		</td>-->
	</tr>
</xsl:template>
</xsl:stylesheet>